import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenditureAuthorizationComponent } from './expenditure-authorization.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LookupService, ResourceRequestService } from '../api/generated';

describe('ExpenditureAuthorizationComponent', () => {
  let component: ExpenditureAuthorizationComponent;
  let fixture: ComponentFixture<ExpenditureAuthorizationComponent>;
  let spyLookupService = jasmine.createSpyObj( { apiLookupLookupTypeGet: null } );
  let spyResourceRequestService = jasmine.createSpyObj( { apiResourceRequestPost: null } );

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenditureAuthorizationComponent ],
      imports: [ ReactiveFormsModule ],
      providers: [ 
        { provide: LookupService, useValue: spyLookupService }, 
        { provide: ResourceRequestService, useValue: spyResourceRequestService }
      ]
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
