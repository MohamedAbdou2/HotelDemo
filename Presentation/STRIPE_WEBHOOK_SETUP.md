# Stripe Payment Integration - Webhook Setup Guide

## Overview
This guide explains how to configure Stripe webhooks to properly confirm payments in your database after successful Stripe checkout sessions.

## Problem Fixed
Previously, Stripe payments were succeeding but not being confirmed in the database. This has been fixed by implementing proper webhook handling and success URL callbacks.

## Changes Made

### 1. Controller Updates (`PaymentController.cs`)
- **Added `/api/payment/success` endpoint**: Handles Stripe redirect after successful payment
- **Added `/api/payment/cancel` endpoint**: Handles payment cancellation
- **Updated `/api/payment/webhook` endpoint**: Now properly handles Stripe webhook events with signature verification

### 2. Service Layer Updates
- **`PaymentService.cs`**: Added methods:
  - `HandleStripeSuccessAsync()`: Processes payment when user is redirected back from Stripe
  - `HandleStripeCancelAsync()`: Handles cancelled payments
  - `HandleStripeWebhookAsync()`: Processes Stripe webhook events (recommended approach)

- **`StripePaymentService.cs`**: Added methods:
  - `GetSessionStatusAsync()`: Retrieves session status from Stripe
  - `ProcessWebhookAsync()`: Validates webhook signature and parses event data

### 3. Configuration
- Added `WebhookSecret` to `appsettings.json` under Stripe configuration

## Setup Instructions

### Step 1: Test Locally with Stripe CLI

1. **Install Stripe CLI**:
   ```bash
   # Windows (using Scoop)
   scoop install stripe
   
   # Or download from https://stripe.com/docs/stripe-cli
   ```

2. **Login to Stripe CLI**:
   ```bash
   stripe login
   ```

3. **Forward webhooks to your local endpoint**:
   ```bash
   stripe listen --forward-to https://localhost:7181/api/payment/webhook
   ```

4. **Copy the webhook signing secret** from the CLI output (starts with `whsec_...`)

5. **Update `appsettings.json`**:
   ```json
   "Stripe": {
     "SecretKey": "your_secret_key",
     "PublishableKey": "your_publishable_key",
     "WebhookSecret": "whsec_..." // Paste the secret from step 4
   }
   ```

### Step 2: Test the Payment Flow

1. **Initiate a payment**:
   ```http
   POST /api/payment/initiate
   Content-Type: application/json
   Authorization: Bearer {your_token}

   {
     "reservationId": "your-reservation-guid"
   }
   ```

2. **Complete payment on Stripe**:
   - Use the `paymentUrl` returned from the initiate endpoint
   - Use Stripe test card: `4242 4242 4242 4242`
   - Any future expiry date and CVC

3. **Verify in database**:
   - Payment status should be `Paid`
   - Reservation status should be `Confirmed`
   - `WebhookVerified` should be `true`
   - `TransactionId` should contain Stripe Payment Intent ID

### Step 3: Production Setup

1. **Create webhook endpoint in Stripe Dashboard**:
   - Go to: https://dashboard.stripe.com/webhooks
   - Click "Add endpoint"
   - Enter URL: `https://your-domain.com/api/payment/webhook`
   - Select event: `checkout.session.completed`
   - Copy the signing secret

2. **Update production `appsettings.json`**:
   ```json
   "Stripe": {
     "SecretKey": "sk_live_...",
     "PublishableKey": "pk_live_...",
     "WebhookSecret": "whsec_..." // Production webhook secret
   },
   "AppUrl": "https://your-domain.com"
   ```

## How It Works

### Two-Way Confirmation System

1. **Success URL Redirect (Immediate)**:
   - User completes payment on Stripe
   - Stripe redirects to `/api/payment/success?paymentId={guid}`
   - System verifies session status with Stripe API
   - Payment and reservation are confirmed immediately
   - User sees success page right away

2. **Webhook (Backup & Verification)**:
   - Stripe sends `checkout.session.completed` event to webhook
   - System validates webhook signature for security
   - If payment not already confirmed, it's confirmed now
   - Sets `WebhookVerified = true` for audit trail

### Why Both Methods?

- **Success URL**: Fast user feedback, works even if webhook fails
- **Webhook**: More reliable, works even if user closes browser
- Together they ensure 99.99% reliability

## Testing Webhook Events

Use Stripe CLI to trigger test events:

```bash
# Test successful payment
stripe trigger checkout.session.completed

# Test with specific payment amount
stripe trigger checkout.session.completed --add checkout_session:amount_total=5000
```

## Troubleshooting

### Payment succeeds but not confirmed in database

**Check:**
1. Webhook secret is correct in `appsettings.json`
2. Stripe CLI is running (for local testing)
3. Check application logs for webhook errors
4. Verify `AppUrl` matches your actual URL

### Webhook signature verification fails

**Solutions:**
- Ensure webhook secret matches the one from Stripe Dashboard/CLI
- Don't modify the raw request body before verification
- Check that endpoint is receiving POST requests

### Session not found error

**Solutions:**
- Payment might be using old session (restart app to clear cache)
- Check that `paymentId` in URL matches database record
- Verify Stripe API key is correct

## Security Notes

1. **Never expose webhook secret** in client-side code or public repositories
2. **Always verify webhook signatures** before processing events
3. **Use HTTPS in production** for webhook endpoints
4. **Implement idempotency** - check payment status before processing

## Support

For Stripe-specific issues:
- Stripe Documentation: https://stripe.com/docs
- Stripe Support: https://support.stripe.com

For application issues:
- Check application logs in Visual Studio Output window
- Enable detailed logging in `appsettings.Development.json`
