import { inject, Injectable } from '@angular/core';
import { CartService } from '../cart/services/cart.service';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  cartService = inject(CartService);

  public items: any[] = [];
  
  constructor() { 
  }

  getHistory(){
    this.items = this.cartService.getBaught();
  }

  getItems(){
    console.log(this.items);
    return this.items;
  }
}
