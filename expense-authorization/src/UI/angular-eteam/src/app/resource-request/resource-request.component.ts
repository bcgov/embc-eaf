import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-resource-request',
  templateUrl: './resource-request.component.html',
  styleUrls: ['./resource-request.component.scss']
})
export class ResourceRequestComponent implements OnInit {

  constructor(private fb: FormBuilder) { }

  resourceRequestForm = this.fb.group({
    dateOfRequest: [''],
    timeOfRequest: [''],
    trackingNumber: [''],
    priority: [''],
    requestorsCommunity: [''],
    requestorsContactInfo: [''],
    resourceCategory: [''],
    resourceType: [''],
    quantity: [''],
    unitsOfMeasure: [''],
    whenRequired: ['']
  });

  ngOnInit(): void {
  }

  onSubmit() {
    // console.log(this.resourceRequestForm.get('dateOfRequest'));
    console.log(this.resourceRequestForm.value);
  }

}
