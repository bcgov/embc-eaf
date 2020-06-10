import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ResourceRequestComponent } from './resource-request/resource-request.component';
import { ExpenditureAuthorizationComponent } from './expenditure-authorization/expenditure-authorization.component';

const routes: Routes = [
  { path: '', redirectTo: '/expenditure-authorization', pathMatch: 'full' },
  { path: 'resource-request', component: ResourceRequestComponent},
  { path: 'expenditure-authorization', component: ExpenditureAuthorizationComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
