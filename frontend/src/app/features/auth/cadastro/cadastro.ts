import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/auth/auth.service';

function senhasIguais(grupo: AbstractControl): ValidationErrors | null {
  const password = grupo.get('password')?.value;
  const rePassword = grupo.get('rePassword')?.value;

  return password === rePassword ? null : { senhasDiferentes: true };
}

@Component({
  selector: 'app-cadastro',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.scss',
})
export class Cadastro {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly enviando = signal(false);
  protected readonly erro = signal<string | null>(null);

  protected readonly formulario = this.fb.nonNullable.group(
    {
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rePassword: ['', Validators.required],
    },
    { validators: senhasIguais },
  );

  protected cadastrar(): void {
    if (this.formulario.invalid) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.enviando.set(true);
    this.erro.set(null);

    const dados = this.formulario.getRawValue();

    this.authService.cadastrar(dados).subscribe({
      next: () => {
        this.authService.login({ username: dados.username, password: dados.password }).subscribe({
          next: () => this.router.navigate(['/dashboard']),
          error: () => this.router.navigate(['/login']),
        });
      },
      error: (erro: Error) => {
        this.erro.set(erro.message);
        this.enviando.set(false);
      },
    });
  }

  protected invalido(campo: 'username' | 'email' | 'password' | 'rePassword'): boolean {
    const controle = this.formulario.controls[campo];
    return controle.invalid && controle.touched;
  }

  protected get senhasDiferentes(): boolean {
    return (
      this.formulario.hasError('senhasDiferentes') && this.formulario.controls.rePassword.touched
    );
  }
}
