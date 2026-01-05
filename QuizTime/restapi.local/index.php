<?php
// Huidige pad
$fullUri = $_SERVER['REQUEST_URI'];
$scriptName = dirname($_SERVER['SCRIPT_NAME']); // bv. /questions
$path = str_replace($scriptName, '', $fullUri);
$path = parse_url($path, PHP_URL_PATH);
$path = rtrim($path, '/');

// Debug info
// echo "Full URI: $fullUri<br>";
// echo "Script folder: $scriptName<br>";
// echo "Pad: $path<br>";

if ($path === '/all') {
    include 'demo_questions.php';
} elseif (preg_match('#^/id/(\d+)$#', $path, $matches)) {
    $id = $matches[1];
    include 'demo_questions.php';

} else {
    http_response_code(404);
    echo "Pagina niet gevonden.";
}
