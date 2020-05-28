# AngularEteam

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 9.1.7.

## Development setup

The ApiModule is generated using `openapi-generator-cli` following the OpenAPI spec.  
The generated content is stored in the `./src/app/api/generated` folder of this project.  
Whenever the API changes, the api must be regenerated and committed to the source code.
To accomplish this:
- copy the current openapi spec from the .NET service into this codebase
    ie. `http://localhost:8080/swagger/v1/swagger.json` -> `./src/app/api/swagger.json`
- run the NPM script `generate:api` and replace all content in `./src/app/api/generated` with the newly generated code
    
NB: Java must be installed in order to use the openapi-generator-cli.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).
