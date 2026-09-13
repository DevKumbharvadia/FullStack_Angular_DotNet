import { Component, inject } from '@angular/core';
import { CartService } from './services/cart.service';
import { ProfileComponent } from '../profile/profile.component';
import { ProfileService } from '../profile/profile.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  cartService = inject(CartService);
  profileService = inject(ProfileService)
  deleteFromCart(item: any){
    this.cartService.delete(item);
  }

}
