import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { LookupService } from '../api/generated/api/lookup.service'

/** 
 * This Component is for a Resource Request Form, an online version of the pdf form available here:
 * https://www2.gov.bc.ca/assets/gov/public-safety-and-emergency-services/emergency-preparedness-response-recovery/local-government/eoc-forms/resource_request.pdf
 */
@Component({
  selector: 'app-resource-request',
  templateUrl: './resource-request.component.html',
  styleUrls: ['./resource-request.component.scss']
})
export class ResourceRequestComponent implements OnInit {

  constructor(private fb: FormBuilder, private lookupService: LookupService) { }

  resourceRequestForm = this.fb.group({
    dateOfRequest: [null],
    timeOfRequest: [null],
    trackingNumber: [null],
    priority: [null],
    requestorsCommunity: [null],
    requestorsContactInfo: [null],
    resourceCategory: [null],
    resourceType: ['', Validators.required],
    quantity: [null, [Validators.required, Validators.pattern("[1-9]+")]],
    unitsOfMeasure: [null],
    whenRequired: [null],
    mission: [null],
    fuel: [null],
    meals: [null],
    operators: [null],
    water: [null],    
    maintenance: [null],
    lodging: [null],
    power: [null],
    location: [null],
    apt: [null],
    streetAddress: [null],
    city: [null],
    province: [null],
    postcode: [null],
    additionalInformation: [null]
  });
  
  ngOnInit(): void {
  }

  get dateOfRequest() { return this.resourceRequestForm.get('dateOfRequest'); }

  onSubmit() {
    // console.log(this.resourceRequestForm.get('dateOfRequest'));
    console.log(this.resourceRequestForm.value);
  }

}
