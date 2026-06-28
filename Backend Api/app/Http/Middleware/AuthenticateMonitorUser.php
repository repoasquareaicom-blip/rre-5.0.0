<?php

namespace App\Http\Middleware;

use App\Services\MonitorAuthService;
use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class AuthenticateMonitorUser
{
  public function __construct(private MonitorAuthService $auth) {}

  public function handle(Request $request, Closure $next): Response
  {
      $user = $this->auth->findUserByToken($request->bearerToken());

      if (! $user) {
          return response()->json(['message' => 'Unauthenticated.'], 401);
      }

      $request->attributes->set('monitor_user', $user);

      return $next($request);
  }
}
