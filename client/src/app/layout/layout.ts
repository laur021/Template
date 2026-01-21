import { Component } from '@angular/core';
import { Header } from "./header/header";
import { Content } from "./content/content";
import { SideNav } from "./side-nav/side-nav";
import { Footer } from "./footer/footer";

@Component({
  selector: 'app-main-layout',
  imports: [Header, Content, SideNav, Footer],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {

}
