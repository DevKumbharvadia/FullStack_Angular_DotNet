import { Component, OnInit } from '@angular/core';
import { Product } from 'src/app/models/product';
import { ProductService } from 'src/app/services/product.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-display-products',
  templateUrl: './display-products.component.html',
})
export class DisplayProductsComponent implements OnInit {
  
  products!:Product[];
  imageBaseUrl=environment.baseUrl+'/resources/';
  constructor(private productService:ProductService) { }
  getProducts(){
    this.productService.getAll().subscribe({
      next:(resp)=>{
        this.products=resp;
      },
      error:(err)=>{
        console.log(err);
      }
    })
  }
  ngOnInit(): void {
    this.getProducts();
  }

}
