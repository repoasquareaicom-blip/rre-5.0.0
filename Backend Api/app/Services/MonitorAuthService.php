<?php

namespace App\Services;

use App\Models\MonitorApiToken;
use App\Models\User;
use Illuminate\Support\Str;

class MonitorAuthService
{
  public const SESSION_HOURS = 12;

  /**
   * @return array{token: string, user: array<string, mixed>, expires_at: string|null}
   */
  public function issueToken(User $user, bool $remember): array
  {
      $plain = Str::random(64);
      $expiresAt = $remember ? null : now()->addHours(self::SESSION_HOURS);

      MonitorApiToken::query()->create([
          'user_id' => $user->id,
          'name' => $remember ? 'remember' : 'session',
          'token_hash' => hash('sha256', $plain),
          'expires_at' => $expiresAt,
      ]);

      return [
          'token' => $plain,
          'user' => $this->userPayload($user),
          'expires_at' => $expiresAt?->toIso8601String(),
      ];
  }

  public function findUserByToken(?string $plain): ?User
  {
      if (! $plain) {
          return null;
      }

      $hash = hash('sha256', $plain);

      $record = MonitorApiToken::query()
          ->where('token_hash', $hash)
          ->where(function ($q) {
              $q->whereNull('expires_at')->orWhere('expires_at', '>', now());
          })
          ->first();

      if (! $record) {
          return null;
      }

      $record->update(['last_used_at' => now()]);

      return User::query()->find($record->user_id);
  }

  public function revokeToken(?string $plain): void
  {
      if (! $plain) {
          return;
      }

      MonitorApiToken::query()
          ->where('token_hash', hash('sha256', $plain))
          ->delete();
  }

  /**
   * @return array<string, mixed>
   */
  public function userPayload(User $user): array
  {
      return [
          'id' => $user->id,
          'name' => $user->name,
          'username' => $user->username,
          'email' => $user->email,
      ];
  }
}
