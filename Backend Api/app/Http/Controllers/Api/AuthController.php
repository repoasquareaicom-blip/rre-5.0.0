<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Services\MonitorAuthService;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Validation\ValidationException;

class AuthController extends Controller
{
  public function __construct(private MonitorAuthService $auth) {}

  public function login(Request $request): JsonResponse
  {
      $validated = $request->validate([
          'username' => ['required', 'string', 'min:2', 'max:64'],
          'password' => ['required', 'string'],
          'remember' => ['sometimes', 'boolean'],
      ]);

      $username = strtolower(trim($validated['username']));

      if (! Auth::attempt([
          'username' => $username,
          'password' => $validated['password'],
      ])) {
          throw ValidationException::withMessages([
              'username' => ['These credentials do not match our records.'],
          ]);
      }

      /** @var \App\Models\User $user */
      $user = Auth::user();
      Auth::logout();

      $payload = $this->auth->issueToken($user, (bool) ($validated['remember'] ?? false));

      return response()->json($payload);
  }

  public function me(Request $request): JsonResponse
  {
      $user = $request->attributes->get('monitor_user');

      return response()->json([
          'user' => $this->auth->userPayload($user),
      ]);
  }

  public function logout(Request $request): JsonResponse
  {
      $this->auth->revokeToken($request->bearerToken());

      return response()->json(['ok' => true]);
  }
}
