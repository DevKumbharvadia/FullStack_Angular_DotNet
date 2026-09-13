import { inject, Injectable } from '@angular/core';
import { AppComponent } from '../../../app.component';

@Injectable({
  providedIn: 'root',
})
export class CartService {

  public items: any[] = [];
  public baught: any[] = [];

  constructor() {}

  addToCart(product: any) {
    this.items.push({ ...product, Quantity: 1 });
  }

  getItems() {
    return this.items;
  }

  delete(item: any) {
    this.items = this.items.filter((i) => i.id !== item.id);
  }

  incrementQuantity(id: number) {
    let item = this.items.find((i) => i.id == id);
    if (item) {
      item.Quantity++;
    }
  }

  decrementQuantity(id: number) {
    let item = this.items.find((i) => i.id == id);
    if (item && item.Quantity > 1) {
      item.Quantity--;
    }
  }

  getTotal() {
    let num = this.items.reduce((acc, item) => {
      return acc + item.price * item.Quantity;
    }, 0);

    return num.toFixed(2);
  }

  Checkout()
  {
    this.baught = this.baught.concat(this.items);
    this.items = [];
  }

  getBaught(){
    return this.baught;
  }
}
