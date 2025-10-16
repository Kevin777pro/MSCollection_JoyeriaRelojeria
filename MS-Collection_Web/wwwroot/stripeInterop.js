// wwwroot/js/stripeInterop.js
let stripe;
let card;

window.stripeInterop = {
    initialize: function (publishableKey) {
        stripe = Stripe(publishableKey);
        const elements = stripe.elements();
        card = elements.create('card');
        card.mount('#card-element');
    },
    confirmCardPayment: async function (clientSecret) {
        const result = await stripe.confirmCardPayment(clientSecret, {
            payment_method: {
                card: card
            }
        });

        if (result.error) {
            return {
                success: false,
                message: result.error.message
            };
        } else {
            return {
                success: true,
                paymentIntentId: result.paymentIntent.id
            };
        }
    }
};
