<?php

namespace App\Models\Concerns;

trait BranchHeaderTable
{
    public function initializeBranchHeaderTable(): void
    {
        $this->timestamps = false;
        $this->incrementing = false;
    }
}
