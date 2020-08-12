import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenditureAuthorizationComponent } from './expenditure-authorization.component';
import { ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { LookupService, ResourceRequestService, LookupValue } from '../api/generated';
import { of } from 'rxjs';

const lookupServiceMockup = {
  apiLookupLookupTypeGet() {
    const communities: LookupValue[] = [
      {id: '1', value: 'Community1'}
    ];
    return of(communities);
  }
};

describe('ExpenditureAuthorizationComponent', () => {
  let component: ExpenditureAuthorizationComponent;
  let fixture: ComponentFixture<ExpenditureAuthorizationComponent>;
  let spyResourceRequestService = jasmine.createSpyObj( { apiResourceRequestPost: null } );

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpenditureAuthorizationComponent ],
      imports: [ ReactiveFormsModule ],
      providers: [ 
        { provide: LookupService, useValue: lookupServiceMockup }, 
        { provide: ResourceRequestService, useValue: spyResourceRequestService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpenditureAuthorizationComponent);
    component = fixture.componentInstance;
    component.ngOnInit();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('form invalid when empty', () => {
    expect(component.expndAuthForm.valid).toBeFalsy();
  });

  it('event field validity', () => {
    let errors = {};
    let evnt = component.expndAuthForm.controls['expEvent'];

    // Event field is required
    errors = evnt.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Event field to something
    evnt.setValue("Some event");
    errors = evnt.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('dateOfRequest field validity', () => {
    let errors = {};
    let dateOfRequest = component.expndAuthForm.controls['dateOfRequest'];

    // Date of Request field is required
    errors = dateOfRequest.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Date of Request field to now
    dateOfRequest.setValue(new Date().toDateString());
    errors = dateOfRequest.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy();
    
    // Set Date of Request field to the future
    dateOfRequest.setValue(addDays(new Date(), 1).toDateString());
    errors = dateOfRequest.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeTruthy();
  });

  it('timeOfRequest field validity', () => {
    let errors = {};
    let timeOfRequest = component.expndAuthForm.controls['timeOfRequest'];
    let dateOfRequest = component.expndAuthForm.controls['dateOfRequest'];

    // Time of Request field is required
    errors = timeOfRequest.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Time of Request field to noon, but leave Date of Request blank
    timeOfRequest.setValue("12:00");
    errors = timeOfRequest.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy(); // doesn't matter if actually in the future or not since date is not set
    
    // Set Time of Request field to noon, Date of Request to yesterday
    dateOfRequest.setValue(addDays(new Date(), -1).toDateString());
    timeOfRequest.setValue("12:00");
    errors = timeOfRequest.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy(); // time not in future since date is yesterday
    
    // Set Time of Request field to noon, Date of Request to tomorrow
    dateOfRequest.setValue(addDays(new Date(), 1).toDateString());
    timeOfRequest.setValue("12:00");
    errors = timeOfRequest.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeTruthy(); // time is in the future
  });

  it('eafNo field validity', () => {
    let errors = {};
    let eafNo = component.expndAuthForm.controls['eafNo'];

    // EAF# field is required
    errors = eafNo.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set EAF# field to something
    eafNo.setValue("Some eafNo");
    errors = eafNo.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('embcTaskNo field validity', () => {
    let errors = {};
    let embcTaskNo = component.expndAuthForm.controls['embcTaskNo'];

    // EMBC Task # field is required
    errors = embcTaskNo.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set EMBC Task # field to something
    embcTaskNo.setValue("Some embcTaskNo");
    errors = embcTaskNo.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('requestingCommunity field validity', () => {
    let errors = {};
    let requestorsCommunity = component.expndAuthForm.controls['requestorsCommunity'];

    // Requesting Organization/Community field is required
    errors = requestorsCommunity.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Requesting Organization/Community field to something
    requestorsCommunity.setValue("Some community");
    errors = requestorsCommunity.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('name field validity', () => {
    let errors = {};
    let repName = component.expndAuthForm.controls['repName'];

    // Authorized Representative Name field is required
    errors = repName.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Name field to something
    repName.setValue("Some name");
    errors = repName.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('telephone field validity', () => {
    let errors = {};
    let repTelephone = component.expndAuthForm.controls['repTelephone'];

    // Authorized Representative Telephone field is required
    errors = repTelephone.errors || {};
    expect(errors['required']).toBeTruthy();
    expect(errors['pattern']).toBeFalsy();

    // test valid email telephone numbers
    expectPattern(true, repTelephone, "123 456 7890");
    expectPattern(true, repTelephone, "123-456-7890");
    expectPattern(true, repTelephone, "123.456.7890");
    expectPattern(true, repTelephone, "1234567890");

    // test invalid email telephone numbers
    expectPattern(false, repTelephone, "123"); // too short
    expectPattern(false, repTelephone, "12345678900"); // too long
    expectPattern(false, repTelephone, "1234 567 890"); // malformed
    expectPattern(false, repTelephone, "123 4567 890"); // malformed
    expectPattern(false, repTelephone, "123.4567890"); // only 1 separator
    expectPattern(false, repTelephone, "123 4567890"); // only 1 separator
    expectPattern(false, repTelephone, "123-456.7890"); // mixed separators
    expectPattern(false, repTelephone, "123a4567890"); // alphanumeric
  });

  it('email field validity', () => {
    let errors = {};
    let repEmail = component.expndAuthForm.controls['repEmail'];

    // Authorized Representative Email field is required
    errors = repEmail.errors || {};
    expect(errors['required']).toBeTruthy();
    expect(errors['pattern']).toBeFalsy();

    // test valid email addresses
    expectPattern(true, repEmail, "simple@example.com");
    expectPattern(true, repEmail, "very.common@example.com");
    expectPattern(true, repEmail, "disposable.style.email.with+symbol@example.com");
    expectPattern(true, repEmail, "other.email-with-hyphen@example.com");
    expectPattern(true, repEmail, "fully-qualified-domain@example.com");
    expectPattern(true, repEmail, "user.name+tag+sorting@example.com");
    expectPattern(true, repEmail, "x@example.com");
    expectPattern(true, repEmail, "example-indeed@strange-example.com");
    expectPattern(true, repEmail, "admin@mailserver1"); // no TLD, although ICANN discourages this
    expectPattern(true, repEmail, "example@s.example");
    expectPattern(true, repEmail, "\" \"@example.org"); // space between quotes
    expectPattern(true, repEmail, "\"john..doe\"@example.org"); // quoted double dot
    expectPattern(true, repEmail, "mailhost!username@example.org");
    expectPattern(true, repEmail, "user%example.com@example.org");
    //expectPattern(true, repEmail, "\";\"@example.org"); // special characters in local part allowed inside quotes 
    
    // test invalid email addresses
    expectPattern(false, repEmail, "abc.example.com"); // no @
    expectPattern(false, repEmail, "a@b@c@example.com"); // multiple @
    expectPattern(false, repEmail, "1234567890123456789012345678901234567890123456789012345678901234+x@example.com"); // local part is longer than 64 characters
    expectPattern(false, repEmail, "i_like_underscore@but_its_not_allow_in_this_part.example.com"); // underscore is not allowed in domain part
    //expectPattern(false, repEmail, ";@example.org"); // special characters not in quotes
  });

  it('description field validity', () => {
    let errors = {};
    let description = component.expndAuthForm.controls['description'];

    // Description of Expenditure field is required
    errors = description.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Description field to something
    description.setValue("Some description");
    errors = description.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('amountRequested field validity', () => {
    let errors = {};
    let amountRequested = component.expndAuthForm.controls['amountRequested'];

    // Amount Requested is required
    errors = amountRequested.errors || {};
    expect(errors['required']).toBeTruthy();

    // test valid amount
    expectPattern(true, amountRequested, "1234");
    expectPattern(true, amountRequested, "12.34");
    expectPattern(true, amountRequested, "$12.34");
    
    // test invalid amount
    expectPattern(false, amountRequested, "$");
    expectPattern(false, amountRequested, "#12.34");
    expectPattern(false, amountRequested, ".34");
    expectPattern(false, amountRequested, "12.3");
    expectPattern(false, amountRequested, "12.345");
    expectPattern(false, amountRequested, "12a34");
    expectPattern(false, amountRequested, "12.34.56");
  });

  it('expenditureNotToExceed field validity', () => {
    let errors = {};
    let expenditureNotToExceed = component.expndAuthForm.controls['expenditureNotToExceed'];

    // Expenditure Not to Exceed is not required
    errors = expenditureNotToExceed.errors || {};
    expect(errors['required']).toBeFalsy();

    // test valid amount
    expectPattern(true, expenditureNotToExceed, "1234");
    expectPattern(true, expenditureNotToExceed, "12.34");
    expectPattern(true, expenditureNotToExceed, "$12.34");
    
    // test invalid amount
    expectPattern(false, expenditureNotToExceed, "$");
    expectPattern(false, expenditureNotToExceed, "#12.34");
    expectPattern(false, expenditureNotToExceed, ".34");
    expectPattern(false, expenditureNotToExceed, "12.3");
    expectPattern(false, expenditureNotToExceed, "12.345");
    expectPattern(false, expenditureNotToExceed, "12a34");
    expectPattern(false, expenditureNotToExceed, "12.34.56");
  });

  it('processedBy field validity', () => {
    let errors = {};
    let processingApprovedBy = component.expndAuthForm.controls['processingApprovedBy'];

    // Approved for Processing by field is required
    errors = processingApprovedBy.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Approved field to something
    processingApprovedBy.setValue("Some name");
    errors = processingApprovedBy.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('processedPosition field validity', () => {
    let errors = {};
    let processingPosition = component.expndAuthForm.controls['processingPosition'];

    // Approved Position field is required
    errors = processingPosition.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Position field to something
    processingPosition.setValue("Some position");
    errors = processingPosition.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('processedDate field validity', () => {
    let errors = {};
    let processingDate = component.expndAuthForm.controls['processingDate'];

    // Approved Date is required
    errors = processingDate.errors || {};
    expect(errors['required']).toBeTruthy();
    
    // Set Date field to now
    processingDate.setValue(new Date().toDateString());
    errors = processingDate.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy();
    
    // Set Date field to the future
    processingDate.setValue(addDays(new Date(), 1).toDateString());
    errors = processingDate.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeTruthy();
  });

  it('processedTime field validity', () => {
    let errors = {};
    let processingDate = component.expndAuthForm.controls['processingDate'];
    let processingTime = component.expndAuthForm.controls['processingTime'];

    // Time field is required
    errors = processingTime.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Time field to noon, but leave Date blank
    processingTime.setValue("12:00");
    errors = processingTime.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy(); // doesn't matter if actually in the future or not since date is not set
    
    // Set Time field to noon, Date to yesterday
    processingDate.setValue(addDays(new Date(), -1).toDateString());
    processingTime.setValue("12:00");
    errors = processingTime.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy(); // time not in future since date is yesterday
    
    // Set Time field to noon, Date to tomorrow
    processingDate.setValue(addDays(new Date(), 1).toDateString());
    processingTime.setValue("12:00");
    errors = processingTime.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeTruthy(); // time is in the future
  });

  it('expenditureBy field validity', () => {
    let errors = {};
    let expenditureApprovedBy = component.expndAuthForm.controls['expenditureApprovedBy'];

    // Expenditure Request Approved by field is required
    errors = expenditureApprovedBy.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Expenditure field to something
    expenditureApprovedBy.setValue("Some name");
    errors = expenditureApprovedBy.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('expenditurePosition field validity', () => {
    let errors = {};
    let expenditurePosition = component.expndAuthForm.controls['expenditurePosition'];

    // Expenditure Position field is required
    errors = expenditurePosition.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Position field to something
    expenditurePosition.setValue("Some position");
    errors = expenditurePosition.errors || {};
    expect(errors['required']).toBeFalsy();
  });

  it('expenditureDate field validity', () => {
    let errors = {};
    let expenditureDate = component.expndAuthForm.controls['expenditureDate'];

    // Expenditure Date is required
    errors = expenditureDate.errors || {};
    expect(errors['required']).toBeTruthy();
    
    // Set Date field to now
    expenditureDate.setValue(new Date().toDateString());
    errors = expenditureDate.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy();
    
    // Set Date field to the future
    expenditureDate.setValue(addDays(new Date(), 1).toDateString());
    errors = expenditureDate.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeTruthy();
  });

  it('expenditureTime field validity', () => {
    let errors = {};
    let expenditureDate = component.expndAuthForm.controls['expenditureDate'];
    let expenditureTime = component.expndAuthForm.controls['expenditureTime'];

    // Time field is required
    errors = expenditureTime.errors || {};
    expect(errors['required']).toBeTruthy();

    // Set Time field to noon, but leave Date blank
    expenditureTime.setValue("12:00");
    errors = expenditureTime.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy(); // doesn't matter if actually in the future or not since date is not set
    
    // Set Time field to noon, Date to yesterday
    expenditureDate.setValue(addDays(new Date(), -1).toDateString());
    expenditureTime.setValue("12:00");
    errors = expenditureTime.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeFalsy(); // time not in future since date is yesterday
    
    // Set Time field to noon, Date to tomorrow
    expenditureDate.setValue(addDays(new Date(), 1).toDateString());
    expenditureTime.setValue("12:00");
    errors = expenditureTime.errors || {};
    expect(errors['required']).toBeFalsy();
    expect(errors['future']).toBeTruthy(); // time is in the future
  });

  function addDays(date: Date, days: number) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
  }

  function expectPattern(shouldBeValid: boolean, control: AbstractControl, value: string) {
    let errors = {};
    control.setValue(value);
    errors = control.errors || {};
    if (shouldBeValid) {
      expect(errors['pattern']).toBeFalsy();
    }
    else {
      expect(errors['pattern']).toBeTruthy();
    }
  }

});
