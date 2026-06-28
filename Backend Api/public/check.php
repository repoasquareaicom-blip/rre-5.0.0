<?php

echo "PHP Working<br>";

echo "PHP Version: " . PHP_VERSION . "<br>";

echo "Current Path: " . __DIR__ . "<br>";

if (file_exists(__DIR__ . '/../vendor/autoload.php')) {
    echo "vendor/autoload.php found<br>";
} else {
    echo "vendor/autoload.php missing<br>";
}

if (file_exists(__DIR__ . '/../.env')) {
    echo ".env found<br>";
} else {
    echo ".env missing<br>";
}