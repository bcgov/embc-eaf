import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ValidatorFn } from '@angular/forms';
import { LookupService } from '../api/generated/api/lookup.service'
import { ResourceRequestService, LookupType, LookupValue } from '../api/generated';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

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

  constructor(private fb: FormBuilder, private lookupService: LookupService, private resourceRequestService: ResourceRequestService) { }

  communities: LookupValue[];
  resourceTypes: LookupValue[];
  files: File[] = [];
  uploadFileErrors: any;
  submission: String = "none";
  today: Date = new Date();

  expndAuthForm = this.fb.group({
    expEvent: [null, Validators.required],
    dateOfRequest: [null],
    timeOfRequest: [null],
    eafNo: [null, Validators.required],
    embcTaskNo: [null, Validators.required],
    requestorsCommunity: ['', Validators.required],
    resourceType: ['', Validators.required],
    repName: [null, Validators.required],
    repTelephone: [null, [Validators.required, Validators.pattern("^\\d{3}([\\.\\- ]?)\\d{3}\\1\\d{4}$")]],
    repEmail: [null, [Validators.required, Validators.pattern("^[^@]{1,64}@[^_@]+$")]],
    description: [null, Validators.required],
    amountRequested: [null, [Validators.required, Validators.pattern("^\\$?\\d+(\\.\\d{2})?$")]],
    expenditureNotToExceed: [null, [Validators.pattern("^\\$?\\d+(\\.\\d{2})?$")]],
    processingApprovedBy: [null, Validators.required],
    processingPosition: [null, Validators.required],
    processingDate: [null],
    processingTime: [null],
    expenditureApprovedBy: [null, Validators.required],
    expenditurePosition: [null, Validators.required],
    expenditureDate: [null],
    expenditureTime: [null]
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
  get resourceType() { return this.expndAuthForm.get('resourceType'); }
  get repName() { return this.expndAuthForm.get('repName'); }
  get repTelephone() { return this.expndAuthForm.get('repTelephone'); }
  get repEmail() { return this.expndAuthForm.get('repEmail'); }
  get description() { return this.expndAuthForm.get('description'); }
  get amountRequested() { return this.expndAuthForm.get('amountRequested'); }
  get expenditureNotToExceed() { return this.expndAuthForm.get('expenditureNotToExceed'); }  
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

    this.lookupService.apiLookupLookupTypeGet(LookupType.LeadAgencyDeptList)
      .subscribe(items => this.communities = items);
    this.lookupService.apiLookupExpenditureAuthorizationResourceTypesGet()
      .subscribe(items => this.resourceTypes = items);
  }

  /**
   * Validates that the control field (a date) is not in the future (can be on or before today)
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

  uploadFile(event: File[]) {
    let totalFileSize = this.totalFileSize();
    this.uploadFileErrors = "";
    for (let i = 0; i < event.length; i++) {
      const file = event[i];
      if ((totalFileSize + file.size ) < 5242880 ) { // 5MB = 5 * 1024 * 1024
        let type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
        if ('|jpg|jpeg|pdf|png|'.indexOf(type) !== -1) {
          this.files.push(file);
          totalFileSize = totalFileSize + file.size;
        }
        else {
          this.uploadFileErrors = "This file type is not permitted: " + file.name;
        }
      }
      else {
        this.uploadFileErrors = "Total file size exceeded";
      }
    }
  }
  
  deleteAttachment(index: any) {
    this.uploadFileErrors = "";
    this.files.splice(index, 1);
  }

  totalFileSize() {
    let total = 0;
    for (let i = 0; i < this.files.length; i++) {
      total = total + this.files[i].size;
    }
    return total;
  }

  /** Log a HeroService message with the MessageService */
  private log(message: string) {
    console.log(message);
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      this.submission = "failure";
      this.log(`${operation} failed: ${error.message}`);
      if (error.error.title) {
        this.log(`  ${error.error.title}`);
        for(let key in error.error.errors) {
          let child = error.error.errors[key];
          if (child.length > 0) {
            console.log('  ' + child[0]);
          }
        }
      }

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  onSubmit() {
    console.log(this.expndAuthForm.value);

    let expEvent = this.expEvent.value;
    let requestDt = new Date(this.dateOfRequest.value + ' ' + this.timeOfRequest.value).toDateString();
    let eafNo = this.eafNo.value;
    let embcTaskNo = this.embcTaskNo.value;
    let requestorsCommunity = this.requestorsCommunity.value;
    let repName = this.repName.value;
    let repTelephone = this.repTelephone.value;
    let repEmail = this.repEmail.value;
    let description = this.description.value;
    let amountRequested = this.amountRequested.value;
    let expenditureNotToExceed = this.expenditureNotToExceed ? this.expenditureNotToExceed.value : '';
    let processingApprovedBy = this.processingApprovedBy.value;
    let processingPosition = this.processingPosition.value;
    let processingDt = new Date(this.processingDate.value + ' ' + this.processingTime.value).toDateString();
    let expenditureApprovedBy = this.expenditureApprovedBy.value;
    let expenditurePosition = this.expenditurePosition.value;
    let expenditureDt = new Date(this.expenditureDate.value + ' ' + this.expenditureTime.value).toDateString();

    // custom field - mission == description + amountRequested
    let mission = description + '\\n' + amountRequested;
    // custom field - requestorsContactInfo == name + telephone + email
    let requestorsContactInfo = 
          'Name: ' + repName + '\\n' 
          'Telephone: ' + repTelephone + '\\n' 
          'Email: ' + repEmail;
    let resourceType = '';

    console.log(expEvent);

    this.resourceRequestService.apiResourceRequestPost(
        null,
        processingDt,
        null, // currentStatus
        expenditureNotToExceed,
        mission,
        null, // priority
        embcTaskNo, // reqTrackNoEmac
        null, // regTrackNoFema
        null, // reqTrackNoState
        eafNo,
        requestorsCommunity,
        requestorsContactInfo,
        null, // resourceCategory
        resourceType,
        requestDt,
        expEvent,
        expenditureNotToExceed,
        processingApprovedBy,
        processingPosition,
        processingDt,
        expenditureApprovedBy,
        expenditurePosition,
        expenditureDt,
        this.files
      )
      .pipe(
        catchError(this.handleError('API post'))
      )
      .subscribe(() => {
        if (this.submission != "failure") {
          this.submission = "success";
          console.log('apiResourceRequestPost returned');
        }
      });
  }

}
