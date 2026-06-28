<?php

return [

    /*
    |--------------------------------------------------------------------------
    | Sync API time limit (seconds)
    |--------------------------------------------------------------------------
    |
    | Branch sync batches can be large; remote MySQL needs more than the default
    | 30s PHP limit. Override with SYNC_MAX_EXECUTION_SECONDS in .env.
    |
    */

    'max_execution_seconds' => (int) env('SYNC_MAX_EXECUTION_SECONDS', 120),

];
