import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from './user.service';

export const catGuard: CanActivateFn = (route, state) => {
  
  const router = inject(Router)
  const service = inject(UserService)

  if(!service.currentUser?.prefercat){
    console.log(service.currentUser)
    router.navigate(['/dog'])
  }
  return true;
};
