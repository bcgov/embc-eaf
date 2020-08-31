import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceRequestComponent } from './resource-request.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LookupService, ExpenseAuthorizationService } from '../api/generated';

describe('ResourceRequestComponent', () => {
  let component: ResourceRequestComponent;
  let fixture: ComponentFixture<ResourceRequestComponent>;
  let spyLookupService = jasmine.createSpyObj( { apiLookupLookupTypeGet: null } );
  let spyExpenseAuthorizationService = jasmine.createSpyObj( { apiExpenseAuthorizationPost: null } );

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceRequestComponent ],
      imports: [ ReactiveFormsModule ],
      providers: [ 
        { provide: LookupService, useValue: spyLookupService }, 
        { provide: ExpenseAuthorizationService, useValue: spyExpenseAuthorizationService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
