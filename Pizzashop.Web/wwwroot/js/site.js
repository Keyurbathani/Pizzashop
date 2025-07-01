function formatAmountWithCommas(amount) {
    return amount.toLocaleString('en-IN');
}

function parseAmountFromString(amountString) {
    return parseFloat(amountString.replace(/,/g, ''));
}