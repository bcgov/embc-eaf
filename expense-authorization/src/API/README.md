

## Configuration

### E Team Configuration

```json
  "ETeam": {
    "Url": "https://server/instance",
    "Username": "",
    "Password": "",
    "ReportTypeName": "ResourceRequest"
  },
```

- ```Url``` Required. URL to the E TEam server and instance. Do not include trailing slahs (/).
- ```Username``` Required. The E Team user to authenicated using
- ```Password``` Required. The E Team user's password
- ```ReportTypeName``` Optional. The report type name to use. Optional, defaults to ```ResourceRequest```

### Email Configuration

```json
  "Email": {
    "Enabled": true,
    "From": "someone@example.com",
    "SmtpServer": "smtp.example.com",
    "Port": 465,
    "Username": "user@example.com",
    "Password": "password"
  },
```

- ```Enabled``` Optional. If not supplied, defaults to ```true```. Set to false to skip sending email which is useful during development.
- ```From``` Required. The email address to send from
- ```SmtpServer``` Required. The SMTP server name
- ```Port``` Optional. The network port the SMTP server is running on. If not supplied, defaults to 465 (SMTP/SSL)
- ```Username``` / ```Password``` Optional. If supplied, the username and password used to authenticate to the SMPT server, otherwise non-authenticated access will be used

### Logging Configuration

See: [Serilog.Settings.Configuration](https://github.com/serilog/serilog-settings-configuration)
