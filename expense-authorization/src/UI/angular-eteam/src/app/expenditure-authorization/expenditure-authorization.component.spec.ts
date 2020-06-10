import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenditureAuthorizationComponent } from './expenditure-authorization.component';

describe('ExpenditureAuthorizationComponent', () => {
  let component: ExpenditureAuthorizationComponent;
  let fixture: ComponentFixture<ExpenditureAuthorizationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenditureAuthorizationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenditureAuthorizationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
