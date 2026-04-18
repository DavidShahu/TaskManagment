import { Injectable } from '@angular/core';
import Swal, { SweetAlertResult } from 'sweetalert2';

@Injectable({ providedIn: 'root' })
export class SweetAlertService {

    confirm(
    title: string,
    message: string,
    confirmText: string = 'Confirm',
    type: 'warning' | 'error' | 'question' = 'warning'
  ): Promise<SweetAlertResult> {
    return Swal.fire({
      title,
      text: message,
      icon: type,
      showCancelButton: true,
      confirmButtonText: confirmText,
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#e53e3e',
      cancelButtonColor: '#718096',
    });
  }

  success(title: string, message?: string): void {
    Swal.fire({
      title,
      text: message,
      icon: 'success',
      timer: 2000,
      showConfirmButton: false,
    });
  }

  error(title: string, message?: string): void {
    Swal.fire({
      title,
      text: message,
      icon: 'error',
      confirmButtonColor: '#667eea'
    });
  }
}