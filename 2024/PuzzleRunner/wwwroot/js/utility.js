window.copyTextToClipboard = function(text) {
    navigator.clipboard.writeText(text).catch(function (error) {
        console.error('Error copying text: ', error);
    });
}