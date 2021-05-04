# embc-eaf
Repository for Expense Authorization Form Webform Development

![ci-expense-authorization-portal-api](https://github.com/pbolduc/embc-expense-auth-form/workflows/ci-expense-authorization-portal-api/badge.svg)
![ci-expense-authorization-portal-ui](https://github.com/pbolduc/embc-expense-auth-form/workflows/ci-expense-authorization-portal-ui/badge.svg)

## Technology Stack Used
| Layer   | Technology | 
| ------- | ------------ |
| Presentation | Angular 9.1 |
| Business Logic | C# - .NET Core 3.1 |
| Web Server | Kestrel |

## Deployment (Local Development)

### Developer Workstation Requirements/Setup
**Recommended Configuration**
- 16 GB RAM
- SSD
- Core i7 7th generation or better
- Windows 10 Pro
- Docker
- S2I
- Git 

## Build (Github Actions)
The following secets need to be setup in your project to push to push to openshift. Note the OCP3 names are deprecated and will be removed once the migration to OCP4 is complete.

| Secret | Description |
| --- | --- |
| OPENSHIFT_EXTERNAL_REGISTRY | The name of the openshift external registry (OCP4) |
| OPENSHIFT_EXTERNAL_REGISTRY_PASS | The password/token of the service account allowed to push to the image registry |
| OPENSHIFT_EXTERNAL_REGISTRY_USER | The username of the service account allowed to push to the image registry, ie builder |
| OPENSHIFT_TOOLS_NAMESPACE | The tools openshift namespace, ie abcdef-tools |
| OPENSHIFT_OCP3_EXTERNAL_REGISTRY | deprecated - The name of the openshift external registry (OCP3) |
| OPENSHIFT_OCP3_EXTERNAL_REGISTRY_PASS | deprecated - The password/token of the service account allowed to push to the image registry |
| OPENSHIFT_OCP3_EXTERNAL_REGISTRY_USER | deprecated - The username of the service account allowed to push to the image registry, ie builder |
| OPENSHIFT_OCP3_TOOLS_NAMESPACE | deprecated - The tools openshift namespace, ie abcdef-tools |

## Deployment (OpenShift)

See (openshift/README.md)

## Configuration

### API

From Docker or OpenShift, these envrionment variables can be used to configure the API.  See [API Readme](expense-authorization/src/API/README.md).

| Env Variable | Description |
| --- | --- |
| ETEAM__URL | Required. URL to the E TEam server and instance. Do not include trailing slahs (/). |
| ETEAM__USERNAME | Required. The E Team user to authenicated using |
| ETEAM__PASSWORD | Required. The E Team user's password |
| ETEAM__REPORTTYPENAME | Optional. The report type name to use. Optional, defaults to ```ResourceRequest``` |
| EMAIL__ENABLED | Optional. If not supplied, defaults to ```true```. Set to false to skip sending email which is useful during development. |
| EMAIL__FROM | Required. The email address to send from |
| EMAIL__SMTPSERVER | Required. The SMTP server name |
| EMAIL__PORT | Optional. The network port the SMTP server is running on. If not supplied, defaults to 465 (SMTP/SSL) |
| EMAIL__USERNAME | Optional. If supplied, the username and password used to authenticate to the SMPT server, otherwise non-authenticated access will be used|
| EMAIL__PASSWORD | Optional. If supplied, the username and password used to authenticate to the SMPT server, otherwise non-authenticated access will be used |
| SPLUNK_URL | Optional. The URL to Splunk. |
| SPLUNK_TOKEN | The Splunk event collector token. |

## License

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

    Copyright 2018 Province of British Columbia

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
