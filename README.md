# HTTPS-Middleware für den Arduino
Dies ist eine einfache Middleware für den Arduino, da dieser keine HTTPS-Requests machen kann. 
Der Arduino kann Anfragen an den Server (Port 5000) stellen, diese werden dann durch den Server ausgeführt und die Response wird
(mit _Content-Type: application/text_ und Status _200_) zurückgegeben. Dies ermöglicht es, im lokalen Netzwerk
über HTTP zu arbeiten, aber den ColaTerminal-Server nur über HTTPS anzusprechen.

Die Anfragen müssen via _POST_ an `/api/request/send` gesendet werden. Dabei ist folgende Payload (_Content-Type application/json_) nötig:
```
{
    "method": "GET"                         // ["GET", "POST", "PUT", "PATCH", "DELETE"]
    "url": "https://example.com/api",
    "payload": {},                          // Kann prinzipiell von jedem Typ sein. Muss nur bei "POST", "PUT" und "PATCH" gesetzt werden.
    "header": [                             // Optional
        ["Some Header", "Some Value"],
        ["Authorization", "Bearer <Token>"] // Ist immer ein Array mit zwei Werten!
    ]
}
```
_Hinweis_: Die vom Aufruf übergebenen Header werden nicht von der API übertragen!