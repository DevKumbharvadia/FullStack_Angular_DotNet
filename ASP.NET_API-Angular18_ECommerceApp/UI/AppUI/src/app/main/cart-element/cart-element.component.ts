import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppComponent } from '../../app.component';
import { CartService } from '../../header/cart/services/cart.service';

// export type Product = {
//   id: number,
//   Quantity: number; // Quantity
//   image: File; // File object (image)
//   name: string; // Product name
//   description: string; // Product description
//   price: number; // Product price
// };

// const dummyImage = new File(['dummy'], 'placeholder.png', { type: 'image/png' });

@Component({
  selector: 'app-cart-element',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart-element.component.html',
  styleUrl: './cart-element.component.css',
})

export class CartElementComponent { 
  cartService = inject(CartService);
  ListItem = inject(AppComponent);

  addToCart(product: any){
    this.cartService.addToCart(product);
  }
}
