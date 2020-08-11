import { ApiModule, LookupService } from './api/generated/';
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
import { AppRoutingModule } from './app-routing.module';
import { ExpenditureAuthorizationComponent } from './expenditure-authorization/expenditure-authorization.component';
import { DragDropDirective } from './drag-drop.directive';
import { FileSizePipe } from './filesize.pipe';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    ResourceRequestComponent,
    ExpenditureAuthorizationComponent,
    DragDropDirective,
    FileSizePipe
  ],
  imports: [
    BrowserModule,
    NgbModule,
    ReactiveFormsModule,
    HttpClientModule,
    ApiModule,
    AppRoutingModule
  ],
  providers: [{ provide: BASE_PATH, useValue: environment.apiEndpoint }],
  bootstrap: [AppComponent]
})
export class AppModule { }
