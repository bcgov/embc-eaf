import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, ValidatorFn } from '@angular/forms';
import { LookupService } from '../api/generated/api/lookup.service'
import { ExpenseAuthorizationService, LookupType, LookupValue } from '../api/generated';
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

  constructor(private fb: FormBuilder, private lookupService: LookupService, private expenseAuthorizationService: ExpenseAuthorizationService) { }

  communities: LookupValue[];
  resourceTypes: LookupValue[];
  files: File[] = [];
  uploadFileErrors: any;
  submission: String = "none";
  submissionID: String = "";
  now: number = Date.now();

  expndAuthForm = this.fb.group({
    expEvent: [null, Validators.required],
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
    processingDate: [null, [Validators.required, this.dateNotFutureValidator()]],
    processingTime: [null, Validators.required],
    expenditureApprovedBy: [null, Validators.required],
    expenditurePosition: [null, Validators.required],
    expenditureDate: [null, [Validators.required, this.dateNotFutureValidator()]],
    expenditureTime: [null, Validators.required]
  }, {
    validator: Validators.compose([
    ])
  });

  get expEvent() { return this.expndAuthForm.get('expEvent'); }
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
    this.lookupService.apiLookupLookupTypeGet(LookupType.LeadAgencyDeptList)
      .subscribe(items => this.communities = items);
    this.lookupService.apiLookupExpenditureAuthorizationResourceTypesGet()
      .subscribe(items => this.resourceTypes = items);
  }

  /**
   * Validates that the control field (a date) is not in the future (can be on or before today)
   */
  dateNotFutureValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: boolean } => {
      let now = new Date();
      if (now < new Date(control.value)) {
        return { 'future': true };
      }
      return {};
    };
  }

  /** Adds an attachment to the files list.  Logs an error if wrong file type or too large (sum of all attachments must be <5MB) */
  addAttachment(event: File[]) {
    let totalFileSize = this.totalFileSizeSum();
    this.uploadFileErrors = "";
    for (let i = 0; i < event.length; i++) {
      const file = event[i];
      if ((totalFileSize + file.size ) < 5242880 ) { // 5MB = 5 * 1024 * 1024
        let type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
        if ('|jpg|jpeg|pdf|png|doc|docx|xls|xlsx|vnd.openxmlformats-officedocument.wordprocessingml.document|vnd.openxmlformats-officedocument.spreadsheetml.sheet|'.indexOf(type) !== -1) {
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
  
  /** Removes the indexed file from the files list. */
  deleteAttachment(index: number) {
    this.uploadFileErrors = "";
    this.files.splice(index, 1);
  }

  /** Returns the sum of all the file sizes. */
  totalFileSizeSum() {
    let total = 0;
    for (let i = 0; i < this.files.length; i++) {
      total = total + this.files[i].size;
    }
    return total;
  }

  /** Back button on the Unsuccessful screen. */
  onBack() {
    this.now = Date.now(); // reset Date/Time on form
    this.submission = 'none';
    this.submissionID = "";
  }

  /** Main submit method.  Called when Submit is clicked. */
  onSubmit() {
    this.log(this.expndAuthForm.value);

    let expEvent = this.expEvent.value;
    let requestTs = new Date(this.now).toISOString();
    let eafNo = this.eafNo.value;
    let embcTaskNo = this.embcTaskNo.value;
    let requestorsCommunity = this.requestorsCommunity.value;
    let resourceType = this.resourceType.value;
    let repName = this.repName.value;
    let repTelephone = this.repTelephone.value;
    let repEmail = this.repEmail.value;
    let description = this.description.value;
    let amountRequested = this.amountRequested.value;
    let expenditureNotToExceed = this.expenditureNotToExceed ? this.expenditureNotToExceed.value : '';
    let processingApprovedBy = this.processingApprovedBy.value;
    let processingPosition = this.processingPosition.value;
    let processingTs = (this.processingDate.value && this.processingTime.value) ? new Date(this.processingDate.value + ' ' + this.processingTime.value).toISOString() : null;
    let expenditureApprovedBy = this.expenditureApprovedBy.value;
    let expenditurePosition = this.expenditurePosition.value;
    let expenditureTs = (this.expenditureDate.value && this.expenditureTime.value) ? new Date(this.expenditureDate.value + ' ' + this.expenditureTime.value).toISOString() : null;

    this.expenseAuthorizationService.apiExpenseAuthorizationPost(
        expEvent,     // Event
        requestTs,    // DateTime
        eafNo,        // EAF #
        embcTaskNo,   // EMBC Task #
        requestorsCommunity, // Requesting Community
        resourceType, // Resource Type
        repName,      // Authorized Representative Name
        repTelephone, // Authorized Representative Telephone
        repEmail,     // Authorized Representative Email
        description,  // Description
        amountRequested, // Amount Requested
        expenditureNotToExceed, // Expenditure Not To Exceed
        processingApprovedBy, // Approved for Processing By
        processingPosition, // Approved Position
        processingTs, // // Approved DateTime
        expenditureApprovedBy, // Expenditure Approved By
        expenditurePosition, // Expenditure Position
        expenditureTs, // Expenditure DateTime
        this.files
      )
      .pipe(
        catchError(this.handleError('API post'))
      )
      .subscribe((response: {id: String}) => {
        if (this.submission != "failure" && this.submission != "validationFailure") {
          this.submission = "success";
          this.submissionID = response.id;
          this.log('apiResourceRequestPost returned');
        }
      });
  }

  /** Log a message to the console. */
  private log(message: string) {
    // for debugging
    // console.log(message);
  }

  /**
   * Handle Http operation that failed.  Scans the errors for relevent validation errors if any.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      if (error.status == 400 && error.error.title == 'One or more validation errors occurred.') {
        // Server validation error (the UI protects against this with it's own validation so this should never happen)
        // But if we are here then there is a decrepency between the UI and server validation checks.
        this.log(`  ${error.error.title}`);
        for(let key in error.error.errors) {
          let errors = error.error.errors[key];
          if (errors.length > 0) {
            this.submission = "validationFailure";
            if (key === 'event') {
              this.expEvent.markAsDirty();
              this.expEvent.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eAFNo') {
              this.eafNo.markAsDirty();
              this.eafNo.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eMBCTaskNo') {
              this.embcTaskNo.markAsDirty();
              this.embcTaskNo.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'requestingOrg') {
              this.requestorsCommunity.markAsDirty();
              this.requestorsCommunity.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'resourceType') {
              this.resourceType.markAsDirty();
              this.resourceType.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'authName') {
              this.repName.markAsDirty();
              this.repName.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'authTelephone') {
              this.repTelephone.markAsDirty();
              this.repTelephone.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'authEmail') {
              this.repEmail.markAsDirty();
              this.repEmail.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'description') {
              this.description.markAsDirty();
              this.description.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'amountRequested') {
              this.amountRequested.markAsDirty();
              this.amountRequested.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'expenditureNotToExceed') {
              this.expenditureNotToExceed.markAsDirty();
              this.expenditureNotToExceed.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eocApprovals.Processing.ApprovedBy') {
              this.processingApprovedBy.markAsDirty();
              this.processingApprovedBy.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eocApprovals.Processing.Position') {
              this.processingPosition.markAsDirty();
              this.processingPosition.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eocApprovals.Processing.ApprovalDateTime') {
              this.processingDate.markAsDirty();
              this.processingDate.setErrors({ fromServer: errors[0]});
              this.processingTime.markAsDirty();
              this.processingTime.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eocApprovals.ExpenditureRequest.ApprovedBy') {
              this.expenditureApprovedBy.markAsDirty();
              this.expenditureApprovedBy.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eocApprovals.ExpenditureRequest.Position') {
              this.expenditurePosition.markAsDirty();
              this.expenditurePosition.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'eocApprovals.ExpenditureRequest.ApprovalDateTime') {
              this.expenditureDate.markAsDirty();
              this.expenditureDate.setErrors({ fromServer: errors[0]});
              this.expenditureTime.markAsDirty();
              this.expenditureTime.setErrors({ fromServer: errors[0]});
            }
            else if (key === 'files') {
              this.uploadFileErrors = errors[0];
            }
            this.log('  ' + errors[0]);
          }
        }
        // if not a field validation error, then some other unknown error occurred.
        if (this.submission != "validationFailure") {
          this.submission = "failure";
          this.log(`${operation} failed: ${error.message}`);
        }
      }
      else {        
        // Not server validation error, but some other unknown internal server error.
        // Display 'try again later' section.
        this.submission = "failure";
        this.log(`${operation} failed: ${error.message}`);
      }

      this.submissionID = "";
      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
