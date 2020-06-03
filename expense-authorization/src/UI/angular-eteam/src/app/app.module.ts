import { ApiModule } from './api/generated/';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';

import { environment } from '../environments/environment';
import { BASE_PATH } from 'src/app/api/generated/variables'
import { AppComponent } from './app.component';
import { ResourceRequestComponent } from './resource-request/resource-request.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';

@NgModule({
  declarations: [
    AppComponent,
    ResourceRequestComponent,
    HeaderComponent,
    FooterComponent
  ],
  imports: [
    BrowserModule,
    NgbModule,
    ReactiveFormsModule,
    HttpClientModule,
    ApiModule
  ],
  providers: [{ provide: BASE_PATH, useValue: environment.apiEndpoint }],
  bootstrap: [AppComponent]
})
export class AppModule { }
