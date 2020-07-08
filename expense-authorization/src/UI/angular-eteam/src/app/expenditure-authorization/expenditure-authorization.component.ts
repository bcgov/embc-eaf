import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators, AbstractControl, ValidatorFn } from '@angular/forms';

/** 
 * This Component is for an Expenditure Authorization Form, an online version of the pdf form available here:
 * https://www2.gov.bc.ca/assets/gov/public-safety-and-emergency-services/emergency-preparedness-response-recovery/local-government/eoc-forms/eoc_expenditure_authorization_form.pdf
 */

@Component({
  selector: 'app-expenditure-authorization',
  templateUrl: './expenditure-authorization.component.html',
  styleUrls: ['./expenditure-authorization.component.scss']
})
export class ExpenditureAuthorizationComponent implements OnInit {

  constructor(private fb: FormBuilder) { }

  today = new Date();
  expndAuthForm = this.fb.group({
    expEvent: [null, Validators.required],
    dateOfRequest: [null],
    timeOfRequest: [null],
    eafNo: [null, Validators.required],
    embcTaskNo: [null, Validators.required],
    requestorsCommunity: ['', Validators.required],
    repName: [null, Validators.required],
    repTelephone: [null, [Validators.required, Validators.pattern("\\d\\d\\d[\.|\-]\\d\\d\\d[\.|\-]\\d\\d\\d\\d")]],
    repEmail: [null, Validators.required],
    description: [null, Validators.required],
    amountRequested: [null, Validators.required],
    expenditureNotToExceed: [null],
    processingApprovedBy: [null, Validators.required],
    processingPosition: [null, Validators.required],
    processingDate: [null],
    processingTime: [null],
    expenditureApprovedBy: [null, Validators.required],
    expenditurePosition: [null, Validators.required],
    expenditureDate: ['2020-06-09', Validators.required],
    expenditureTime: ['10:55', Validators.required]
  }, {
    validator: Validators.compose([
    ])
  });

  get expEvent() { return this.expndAuthForm.get('expEvent'); }
  get dateOfRequest() { return this.expndAuthForm.get('dateOfRequest'); }
  get timeOfRequest() { return this.expndAuthForm.get('timeOfRequest'); }
  get eafNo() { return this.expndAuthForm.get('eafNo'); }
  get embcTaskNo() { return this.expndAuthForm.get('embcTaskNo'); }
  get requestorsCommunity() { return this.expndAuthForm.get('requestorsCommunity'); }
  get repName() { return this.expndAuthForm.get('repName'); }
  get repTelephone() { return this.expndAuthForm.get('repTelephone'); }
  get repEmail() { return this.expndAuthForm.get('repEmail'); }
  get description() { return this.expndAuthForm.get('description'); }
  get amountRequested() { return this.expndAuthForm.get('amountRequested'); }
  get processingApprovedBy() { return this.expndAuthForm.get('processingApprovedBy'); }
  get processingPosition() { return this.expndAuthForm.get('processingPosition'); }
  get processingDate() { return this.expndAuthForm.get('processingDate'); }
  get processingTime() { return this.expndAuthForm.get('processingTime'); }
  get expenditureApprovedBy() { return this.expndAuthForm.get('expenditureApprovedBy'); }
  get expenditurePosition() { return this.expndAuthForm.get('expenditurePosition'); }
  get expenditureDate() { return this.expndAuthForm.get('expenditureDate'); }
  get expenditureTime() { return this.expndAuthForm.get('expenditureTime'); }

  ngOnInit(): void {
    // the validation for these date/time fields are dependant on each other, so they are defined here instead.
    this.dateOfRequest.setValidators([Validators.required, this.dateNotFutureValidator('timeOfRequest')]);
    this.timeOfRequest.setValidators([Validators.required, this.timeNotFutureValidator('dateOfRequest')]);
    this.processingDate.setValidators([Validators.required, this.dateNotFutureValidator('processingTime')]);
    this.processingTime.setValidators([Validators.required, this.timeNotFutureValidator('processingDate')]);
    this.expenditureDate.setValidators([Validators.required, this.dateNotFutureValidator('expenditureTime')]);
    this.expenditureTime.setValidators([Validators.required, this.timeNotFutureValidator('expenditureDate')]);
  }

  onSubmit() {
    // console.log(this.expndAuthForm.get('dateOfRequest'));
    console.log(this.expndAuthForm.value);
  }

  /**
   * Validates that the control field (a date) is not in the future (can be no or before today)
   * @param tmField related time field that is dependant on this control field and should be fired afterwards to keep current.
   */
  dateNotFutureValidator(tmField: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } => {
      let now = new Date();
      if (now < new Date(control.value)) {
        return { 'future': true };
      }
      let tm = this.expndAuthForm.get(tmField);
      tm.updateValueAndValidity();
      return {};
    };
  }

  /**
   * Validates that the control field (a time) along with the given date field is not in the future (can be on or before now)
   * @param tmField related date field together with the current control field makes up a valid datetime object.
   */
  timeNotFutureValidator(dtField: string): ValidatorFn {
    return (tm: AbstractControl): { [key: string]: boolean } => {
      let dt = this.expndAuthForm.get(dtField);
      let now = new Date();
      let dateTime = new Date(dt.value + ' ' + tm.value);
      if (now < dateTime) {
        return { 'future': true };
      }
      return {};
    };
  }

}
