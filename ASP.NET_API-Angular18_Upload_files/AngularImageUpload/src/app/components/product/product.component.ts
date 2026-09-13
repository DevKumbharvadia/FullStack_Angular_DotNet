import { Component, OnInit } from '@angular/core';
import { ProductService } from 'src/app/services/product.service';
import {FormBuilder,FormGroup,Validators} from '@angular/forms';
import { Product } from 'src/app/models/product';
import { Status } from 'src/app/models/status';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
})
export class ProductComponent implements OnInit {
  frm!:FormGroup;
  // product:Product = {id:0,productName:'',productImage:''};
  imageFile?:File;
  status!:Status;
  constructor(private productService:ProductService,
    private fb:FormBuilder
    ) { }

    get f(){
      return this.frm.controls;
    }
    onPost(){
      this.status= {statusCode:0,message:'wait..'};
 
      const frmData:Product= Object.assign(this.frm.value);
      frmData.imageFile=this.imageFile;
      // we will call our service, and pass this object to it
      this.productService.add(frmData).subscribe({
        next:(res)=>{
          this.status=res;
        },
        error: (err)=>{
         this.status= {statusCode:0,message:'Error on server side'}
          console.log(err);
        }
      })
    }

    onChange(event:any){
     this.imageFile=event.target.files[0];
    }

  ngOnInit(): void {
    this.frm = this.fb.group({
      'id':[0],
      'productName':['',Validators.required],
      'imageFile':[]
    })
  }

}
