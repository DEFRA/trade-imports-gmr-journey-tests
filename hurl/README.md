# hurl-requests

A collection of [hurl](https://hurl.dev/) requests.

## Usage

### Retrieve data from BTMS test environment

**Note:** IPAFFS pre environment is connected to BTMS test environment.

Get a customs declaration as follows:

```bash
hurl --secret access_token="BTMS_ACCESS_TOKEN" --variable id="ID" customs-declaration.hurl
```

Get an import pre notification as follows:

```bash
hurl --secret access_token="BTMS_ACCESS_TOKEN" --variable reference_number="REFERENCE_NUMBER" import-pre-notification.hurl
```
