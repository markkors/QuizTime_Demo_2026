<?php
header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json; charset=UTF-8");

// not allow to direct access
if (basename($_SERVER['PHP_SELF']) === basename(__FILE__)) {
    http_response_code(403);
    echo json_encode(["message" => "Direct access not allowed"]);
    exit;
}

$method = $_SERVER['REQUEST_METHOD'];

// demo data
$quiz = [
    [
        "id" => 1,
        "question" => "Wat is de hoofdstad van Frankrijk?",
        "options" => ["Parijs", "Lyon", "Marseille", "Nice"],
        "answer" => 0
    ],
    [
        "id" => 2,
        "question" => "Welke planeet staat het dichtst bij de zon?",
        "options" => ["Aarde", "Venus", "Mercurius", "Mars"],
        "answer" => 2
    ]
];


if ($method === 'GET') {

    if (isset($id) && is_numeric($id)) {
        // Hier zou je normaal gesproken de quizvraag ophalen uit een database
        // Voor deze demo gebruiken we een mockup
        $quiz = array_values(array_filter($quiz, function($q) use ($id) {
            return $q['id'] == $id;
        }));

        // check of quizvraag bestaat
        if (empty($quiz)) {
            http_response_code(404); // Not Found
            echo json_encode(["message" => "Quiz question not found"]);
            exit;
        }
        http_response_code(200); // OK
        echo json_encode($quiz, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
        exit;
    } else {
        // alle quizvragen ophalen
        // normaal gesproken zou je hier een database-query doen
        // nu even een mockup
        http_response_code(200); // OK
        echo json_encode($quiz, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
    }
} else {
    http_response_code(405); // Method Not Allowed
    echo json_encode(["message" => "Method not allowed"]);
}
