import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders, HttpStatusCode } from '@angular/common/http';
import { NgIf } from '@angular/common';
import { jwtDecode} from 'jwt-decode';
import { Observable } from 'rxjs';
import { Credentials } from '../../Entity/Credentials';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule,ReactiveFormsModule,NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  
  
  constructor(private formBuilder: FormBuilder, private http : HttpClient) { }
  login!: FormGroup
 

  headersAuth = new HttpHeaders({
    'Content-Type': 'application/json',
    responseType: 'text',
  });

  credentials!: Credentials
  isLogged!: boolean 
  
  jwtToken: any



  ngOnInit(): void {
    if(localStorage.getItem('token') != null){
      this.setJwtAuthInfo(true, localStorage.getItem('token')!);
    }
    this.login = this.formBuilder.group({
      email: [''],
      password: [''],
    })

  }

  loginPost(credentials: Credentials): Observable<any> {
    return this.http.post('https://localhost:7233/LoginJwt/Login', credentials, {
      observe: 'response',
    });
  }
  runLogin(){
    this.credentials = new Credentials(this.login.value.email, this.login.value.password);
    
    this.loginPost(this.credentials).subscribe({
      
      next: (response: any) => {
        console.log(response.status);
        
        switch (response.status) {
          case HttpStatusCode.Ok:
            console.log('Login effettuato ');
            this.jwtToken = response.body.token;
            this.setJwtAuthInfo(true, this.jwtToken);
            console.log(jwtDecode(this.jwtToken));
            break;
          case HttpStatusCode.NoContent:
            console.log('Senza risposta');
            break;
        }
      },
    });
    this.login.reset();
  }


  runLogout(){
    this.isLogged = false;
    localStorage.removeItem('token');
    this.headersAuth= this.headersAuth.set('Authorization', '');
    this.login.reset();
  }

  setJwtAuthInfo(isLogged : boolean, token : string){
  this.isLogged = isLogged;
  localStorage.setItem('token',token); 
  this.headersAuth= this.headersAuth.set('Authorization', 'Bearer ' + token);
  
  }

  loadUsers(){
    console.log(this.headersAuth);
    this.http.get('https://localhost:7233/Users/Index',{headers:this.headersAuth}).subscribe(
      (data :any) => {
        console.log(data);
        
      })
  }
  loadUser(){
    console.log(this.headersAuth);
    this.http.get('https://localhost:7233/Users/Details/1',{headers:this.headersAuth}).subscribe(
      (data :any) => {
        console.log(data);
        
      })
  }
}