import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./header/header.component";
import { MainComponent } from "./main/main.component";
import { FooterComponent } from "./footer/footer.component";
import { CartElementComponent } from "./main/cart-element/cart-element.component";
import { CartService } from './header/cart/services/cart.service';
import { CartComponent } from './header/cart/cart.component';

export type Product = {
  id: number; // Unique identifier for the product
  Quantity: number; // Quantity of the product
  name: string; // Name of the product
  description: string; // Description of the product
  price: number; // Price of the product
  // image: File; // Optional: Uncomment if you need to include an image
};

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, MainComponent, FooterComponent, RouterLink, CartElementComponent, CartComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title(title: any) {
    throw new Error('Method not implemented.');
  }
  cartService = inject(CartService);
  
  items: Product[] = [
    {
      id: 1,
      Quantity: 5,
      name: 'Product One',
      description: 'This is a brief description for Product One.',
      price: 19.99,
      // image: new File(['dummy'], 'product1.png', { type: 'image/png' }), // Example placeholder
    },
    {
      id: 2,
      Quantity: 1,
      name: 'Product Two',
      description: 'This is a brief description for Product Two.',
      price: 29.99,
      // image: new File(['dummy'], 'product2.png', { type: 'image/png' }), // Example placeholder
    },
    {
      id: 3,
      Quantity: 1,
      name: 'Product Three',
      description: 'This is a brief description for Product Three.',
      price: 39.99,
      // image: new File(['dummy'], 'product3.png', { type: 'image/png' }), // Example placeholder
    },
  ];

  addToCart(product: any)
  {
    this.cartService.addToCart(product);
  }

}
