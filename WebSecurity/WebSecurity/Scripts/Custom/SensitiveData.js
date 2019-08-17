$(document).ready(function () {
    
});

function SymmetricEncryptionSuccess(model) {
    $('#SymmetricDiv').find('#PlainText').val(model.PlainText);
    $('#SymmetricDiv').find('#PrivateKey').val(model.PrivateKey);
    $('#SymmetricDiv').find('#InitializationVector').val(model.InitializationVector);
    $('#SymmetricDiv').find('#EncryptedText').val(model.EncryptedText);
}

function AsymmetricEncryptionSuccess(model) {
    $('#AsymmetricDiv').find('#PlainText').val(model.PlainText);
    $('#AsymmetricDiv').find('#PrivateKey').val(model.PrivateKey);
    $('#AsymmetricDiv').find('#PublicKey').val(model.PublicKey);
    $('#AsymmetricDiv').find('#EncryptedText').val(model.EncryptedText);
}