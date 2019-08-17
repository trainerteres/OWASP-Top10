using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Security;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace WebSecurity.ReverseEngineering
{
    public class CookieExtractor
    {
        private static double PeriodOfvalidityInMinutes = 43200;

        public const string CookieName = "WebSecurityCookie";
        public const string AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie;
        public const string IsPersistent = "isPersistent";
        public const string RememberBrowser = "rememberBrowser";
        public const string Claim_LastIssuedOn = "LastIssuedOn";

        public static Claim[] ExtractClaimsFromCookie(string ticket, ref string authType)
        {
            ticket = ticket.Replace('-', '+').Replace('_', '/');

            var padding = 3 - ((ticket.Length + 3) % 4);
            if (padding != 0)
                ticket = ticket + new string('=', padding);

            var bytes = Convert.FromBase64String(ticket);

            bytes = System.Web.Security.MachineKey.Unprotect(bytes,
                "Microsoft.Owin.Security.Cookies.CookieAuthenticationMiddleware",
                    "ApplicationCookie", "v1");

            using (var memory = new MemoryStream(bytes))
            {
                using (var compression = new GZipStream(memory,
                                                    CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReader(compression))
                    {
                        var intValue = reader.ReadInt32();
                        string authenticationType = reader.ReadString();
                        var strValue = reader.ReadString();
                        strValue = reader.ReadString();

                        int count = reader.ReadInt32();

                        var claims = new Claim[count];
                        for (int index = 0; index != count; ++index)
                        {
                            string type = reader.ReadString();
                            type = type == "\0" ? ClaimTypes.Name : type;

                            string value = reader.ReadString();

                            string valueType = reader.ReadString();
                            valueType = valueType == "\0" ?
                                           "http://www.w3.org/2001/XMLSchema#string" :
                                             valueType;

                            string issuer = reader.ReadString();
                            issuer = issuer == "\0" ? "LOCAL AUTHORITY" : issuer;

                            string originalIssuer = reader.ReadString();
                            originalIssuer = originalIssuer == "\0" ?
                                                         issuer : originalIssuer;

                            claims[index] = new Claim(type, value,
                                                   valueType, issuer, originalIssuer);
                        }

                        //var identity = new ClaimsIdentity(claims, authenticationType,
                        //                              ClaimTypes.Name, ClaimTypes.Role);

                        //var principal = new ClaimsPrincipal(identity);

                        //var isAuth = principal.Identity.IsAuthenticated;

                        //principal.Identity.IsAuthenticated = false;
                        authType = authenticationType;
                        return claims;
                    }
                }
            }
        }


        public static HttpCookie CreateCookie(Claim[] claims, string authenticationType)
        {
            var identity = new ClaimsIdentity(claims, authenticationType, ClaimTypes.Name, ClaimTypes.Role);

            //Login occured using 'normal' path and IsPersistant was set - generate RememberMeToken cookie
            //var claimsToAdd = GenerateSerializableClaimListFromIdentity(identity);


            //using (var memory = new MemoryStream(bytes))
            //{
            //    using (var compression = new GZipStream(memory, CompressionMode.Compress))
            //    {
            var content = string.Empty;

            List<byte> byteList = new List<byte>();

            content = "3";
            content += "ApplicationCookie";
            content += "\0";
            content += "\0";
            content += "4";

            for (int index = 0; index < claims.Length; index++)
            {
                string type = claims[index].Type;
                content += type == ClaimTypes.Name ? "\0" : type;

                string value = claims[index].Value;
                content += value;

                string valueType = claims[index].ValueType;
                valueType = valueType ==
                               "http://www.w3.org/2001/XMLSchema#string" ? "\0" :
                                 valueType;
                content += valueType;

                string issuer = claims[index].Issuer;
                issuer = issuer == "LOCAL AUTHORITY" ? "\0" : issuer;
                content += issuer;

                string originalIssuer = claims[index].OriginalIssuer;
                originalIssuer = originalIssuer == issuer ? "\0" : originalIssuer;
                content += originalIssuer;
            }

            var bytes = Encoding.ASCII.GetBytes(content);

            MemoryStream output = new MemoryStream();
            byte[] compressed;
            using (var compressionStream = new GZipStream(output, CompressionMode.Compress))
            {
                compressionStream.Write(bytes, 0, bytes.Length);
            }

            compressed = output.ToArray();

            bytes = System.Web.Security.MachineKey.Protect(bytes,
                "Microsoft.Owin.Security.Cookies.CookieAuthenticationMiddleware",
                    "ApplicationCookie", "v1");

            var ticket = Convert.ToBase64String(bytes);

            //var padding = 3 - ((ticket.Length + 3) % 4);

            //if (padding == 0)
            //    ticket = ticket + new string('=', padding);

            ticket = ticket.Replace('+', '-').Replace('/', '_');

            //SerializableClaim cookieExpirationDate = GenerateRememberTokenExpirationDateClaim();

            //claimsToAdd.Add(cookieExpirationDate);

            //var allClaimsInFinalCompressedAndProtectedBase64Token = GenerateProtectedAndBase64EncodedClaimsToken(claimsToAdd);

            HttpCookie cookie = new HttpCookie(".AspNet.ApplicationCookie", ticket);
            cookie.Expires = DateTime.Now.AddMinutes(PeriodOfvalidityInMinutes);
            return cookie;

        }

        public static string CreateCookieValueFromTicket(AuthenticationTicket authTicket)
        {
            var claimsToAdd = GenerateSerializableClaimListFromIdentity(authTicket.Identity);

            SerializableClaim cookieExpirationDate = GenerateRememberTokenExpirationDateClaim();

            claimsToAdd.Add(cookieExpirationDate);

            var ticket = GenerateProtectedAndBase64EncodedClaimsToken(claimsToAdd);

            return ticket;
        }

        public static AuthenticationTicket CreateTicketFromCookie(string ticket, string authenticationType, AuthenticationProperties authProperties)
        {
            var serializableClaims = GetClaimsFromTicket(ticket);

            var claims = serializableClaims.Select(x => new Claim(x.Type, x.Value, x.ValueType));

            var identity = new ClaimsIdentity(claims, authenticationType, ClaimTypes.Name, ClaimTypes.Role);

            var authTicket = new AuthenticationTicket(identity, authProperties);

            return authTicket;
        }


        /// <summary>
        /// Generates serializable collection of user claims, that will be saved inside the cookie token. Custom class is used because Claim class causes 'Circular Reference Exception.'
        /// </summary>
        private static List<SerializableClaim> GenerateSerializableClaimListFromIdentity(ClaimsIdentity identity)
        {
            var dataToReturn = identity.Claims.Select(x =>
                                new SerializableClaim()
                                {
                                    Type = x.Type,
                                    ValueType = x.ValueType,
                                    Value = x.Value
                                }).ToList();

            return dataToReturn;
        }

        /// <summary>
        /// Generates a special claim containing an expiration date of RememberMeToken cookie. This is necessary because we CANNOT rely on browsers here - since each one threat cookies differently
        /// </summary>
        private static SerializableClaim GenerateRememberTokenExpirationDateClaim()
        {
            SerializableClaim cookieExpirationDate = new SerializableClaim()
            {
                Type = "TokenExpirationDate",
                Value = DateTime.Now.AddMinutes(PeriodOfvalidityInMinutes).ToBinary().ToString()
            };
            return cookieExpirationDate;
        }

        /// <summary>
        /// Generates token containing user claims. The token is compressed, encrypted using machine key and returned as base64 string - this string will be saved inside RememberMeToken cookie
        /// </summary>
        private static string GenerateProtectedAndBase64EncodedClaimsToken(List<SerializableClaim> claimsToAdd)
        {
            var allClaimsAsString = JsonConvert.SerializeObject(claimsToAdd);

            var allClaimsAsBytes = Encoding.UTF8.GetBytes(allClaimsAsString);

            var allClaimsAsCompressedBytes = CompressionHelper.CompressDeflate(allClaimsAsBytes);

            var allClaimsAsCompressedBytesProtected = MachineKey.Protect(allClaimsAsCompressedBytes, "CookieAuthentication");

            var allClaimsInFinalCompressedAndProtectedBase64Token = Convert.ToBase64String(allClaimsAsCompressedBytesProtected);

            return allClaimsInFinalCompressedAndProtectedBase64Token;
        }

        public static List<SerializableClaim> GetClaimsFromTicket(string ticket)
        {
            var compressedBytes = Convert.FromBase64String(ticket);

            var protectedBytes = MachineKey.Unprotect(compressedBytes, "CookieAuthentication");

            var unprotectedBytes = CompressionHelper.DecompressDeflate(protectedBytes);

            var claimsJson = Encoding.UTF8.GetString(unprotectedBytes);

            var serializableClaims = JsonConvert.DeserializeObject<IEnumerable<SerializableClaim>>(claimsJson);

            return serializableClaims.ToList();
        }
    }

    public static class CompressionHelper
    {
        public static byte[] CompressDeflate(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] DecompressDeflate(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }

    public class SerializableClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }
}