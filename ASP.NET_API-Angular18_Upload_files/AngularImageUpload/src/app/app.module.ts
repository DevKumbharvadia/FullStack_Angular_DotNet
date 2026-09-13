import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductComponent } from './components/product/product.component';
import { DisplayProductsComponent } from './components/display-products/display-products.component';

@NgModule({
  declarations: [
    AppComponent,
    ProductComponent,
    DisplayProductsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule, // http client call
    ReactiveFormsModule  // for using reactive forms
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
