import { Component } from '@angular/core';
import { CartElementComponent } from "./cart-element/cart-element.component";

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [CartElementComponent],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent {

}
