const Operations = {
    UpdateShippingAddressOperation: 'payments/update/shippingAddress',
    UpdateShippingOptionOperation: 'payments/update/shippingOption',
    PaymentCompleteOperation: 'payments/complete'
};

const PaymentActionType = 'payment';

const MicrosoftPayMethodName = 'https://pay.microsoft.com/microsoftpay';

module.exports = {
    Operations: Operations,
    PaymentActionType: PaymentActionType,
    MicrosoftPayMethodName: MicrosoftPayMethodName
};