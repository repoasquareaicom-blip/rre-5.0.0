<?php

/**
 * @deprecated Use config/sync_mirror.php. Kept for backward compatibility.
 */
return [
    'tables' => array_intersect_key(
        config('sync_mirror.tables', []),
        array_flip(['QuotationHeader', 'QuotationDetails'])
    ),
];
