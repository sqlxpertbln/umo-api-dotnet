#!/bin/bash
# UMO API Seed Data Script
# Lädt mindestens 10 Datensätze in jeden Datenbereich über die API
# Wird nach jedem Deployment ausgeführt

API_BASE="${API_BASE_URL:-https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net}"

echo "=== UMO API Seed Data Script ==="
echo "API Base URL: $API_BASE"
echo "Startzeit: $(date)"

# Funktion zum Prüfen ob Daten bereits existieren
check_data_exists() {
    local endpoint=$1
    local count=$(curl -s "$API_BASE$endpoint" | python3 -c "import sys,json; d=json.load(sys.stdin); print(len(d) if isinstance(d,list) else d.get('totalCount',0))" 2>/dev/null || echo "0")
    echo $count
}

# Funktion zum POST von Daten
post_data() {
    local endpoint=$1
    local data=$2
    curl -s -X POST "$API_BASE$endpoint" \
        -H "Content-Type: application/json" \
        -d "$data" > /dev/null 2>&1
}

echo ""
echo "=== Prüfe vorhandene Daten ==="
CLIENTS_COUNT=$(check_data_exists "/clients")
DEVICES_COUNT=$(check_data_exists "/devices")
DIRECT_PROVIDERS_COUNT=$(check_data_exists "/directProvider")
PROF_PROVIDERS_COUNT=$(check_data_exists "/professionalProvider")
EMERGENCY_DEVICES_COUNT=$(check_data_exists "/ServiceHub/devices")

echo "Klienten: $CLIENTS_COUNT"
echo "Geräte: $DEVICES_COUNT"
echo "Direct Provider: $DIRECT_PROVIDERS_COUNT"
echo "Professional Provider: $PROF_PROVIDERS_COUNT"
echo "Notrufgeräte: $EMERGENCY_DEVICES_COUNT"

# ============================================
# KLIENTEN (ClientDetails)
# ============================================
if [ "$CLIENTS_COUNT" -lt 10 ]; then
    echo ""
    echo "=== Erstelle Klienten ==="
    
    # Klient 1
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1001",
        "titleId": 1,
        "firstName": "Max",
        "lastName": "Mustermann",
        "birthDate": "1960-05-15",
        "gender": "M",
        "street": "Musterstraße",
        "houseNumber": "1",
        "zipCode": "10115",
        "city": "Berlin",
        "country": "Deutschland",
        "phone": "+49 30 12345678",
        "mobile": "+49 170 1234567",
        "email": "max.mustermann@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 1 erstellt: Max Mustermann"
    
    # Klient 2
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1002",
        "titleId": 2,
        "firstName": "Erika",
        "lastName": "Musterfrau",
        "birthDate": "1955-08-22",
        "gender": "F",
        "street": "Beispielweg",
        "houseNumber": "42",
        "zipCode": "80331",
        "city": "München",
        "country": "Deutschland",
        "phone": "+49 89 98765432",
        "mobile": "+49 171 9876543",
        "email": "erika.musterfrau@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 2 erstellt: Erika Musterfrau"
    
    # Klient 3
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1003",
        "titleId": 1,
        "firstName": "Hans",
        "lastName": "Meier",
        "birthDate": "1945-03-10",
        "gender": "M",
        "street": "Hauptstraße",
        "houseNumber": "15",
        "zipCode": "50667",
        "city": "Köln",
        "country": "Deutschland",
        "phone": "+49 221 5551234",
        "mobile": "+49 172 5551234",
        "email": "hans.meier@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 3 erstellt: Hans Meier"
    
    # Klient 4
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1004",
        "titleId": 2,
        "firstName": "Helga",
        "lastName": "Schulz",
        "birthDate": "1938-07-22",
        "gender": "F",
        "street": "Gartenweg",
        "houseNumber": "8",
        "zipCode": "20095",
        "city": "Hamburg",
        "country": "Deutschland",
        "phone": "+49 40 3334455",
        "mobile": "+49 173 3334455",
        "email": "helga.schulz@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 4 erstellt: Helga Schulz"
    
    # Klient 5
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1005",
        "titleId": 1,
        "firstName": "Werner",
        "lastName": "Fischer",
        "birthDate": "1952-11-05",
        "gender": "M",
        "street": "Lindenallee",
        "houseNumber": "23",
        "zipCode": "60311",
        "city": "Frankfurt",
        "country": "Deutschland",
        "phone": "+49 69 7778899",
        "mobile": "+49 174 7778899",
        "email": "werner.fischer@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 5 erstellt: Werner Fischer"
    
    # Klient 6
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1006",
        "titleId": 2,
        "firstName": "Ingrid",
        "lastName": "Wagner",
        "birthDate": "1948-02-28",
        "gender": "F",
        "street": "Rosenstraße",
        "houseNumber": "5",
        "zipCode": "70173",
        "city": "Stuttgart",
        "country": "Deutschland",
        "phone": "+49 711 2223344",
        "mobile": "+49 175 2223344",
        "email": "ingrid.wagner@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 6 erstellt: Ingrid Wagner"
    
    # Klient 7
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1007",
        "titleId": 1,
        "firstName": "Klaus",
        "lastName": "Becker",
        "birthDate": "1940-09-15",
        "gender": "M",
        "street": "Kirchplatz",
        "houseNumber": "12",
        "zipCode": "40213",
        "city": "Düsseldorf",
        "country": "Deutschland",
        "phone": "+49 211 4445566",
        "mobile": "+49 176 4445566",
        "email": "klaus.becker@example.de",
        "statusId": 2,
        "languageId": 1
    }'
    echo "Klient 7 erstellt: Klaus Becker"
    
    # Klient 8
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1008",
        "titleId": 2,
        "firstName": "Gerda",
        "lastName": "Hoffmann",
        "birthDate": "1935-12-01",
        "gender": "F",
        "street": "Waldweg",
        "houseNumber": "7",
        "zipCode": "04109",
        "city": "Leipzig",
        "country": "Deutschland",
        "phone": "+49 341 6667788",
        "mobile": "+49 177 6667788",
        "email": "gerda.hoffmann@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 8 erstellt: Gerda Hoffmann"
    
    # Klient 9
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1009",
        "titleId": 1,
        "firstName": "Friedrich",
        "lastName": "Zimmermann",
        "birthDate": "1958-04-20",
        "gender": "M",
        "street": "Bergstraße",
        "houseNumber": "33",
        "zipCode": "01067",
        "city": "Dresden",
        "country": "Deutschland",
        "phone": "+49 351 8889900",
        "mobile": "+49 178 8889900",
        "email": "friedrich.zimmermann@example.de",
        "statusId": 1,
        "languageId": 1
    }'
    echo "Klient 9 erstellt: Friedrich Zimmermann"
    
    # Klient 10
    post_data "/clientdetails" '{
        "mandantId": 1,
        "clientNumber": "1010",
        "titleId": 2,
        "firstName": "Ursula",
        "lastName": "Braun",
        "birthDate": "1942-06-08",
        "gender": "F",
        "street": "Seeweg",
        "houseNumber": "18",
        "zipCode": "28195",
        "city": "Bremen",
        "country": "Deutschland",
        "phone": "+49 421 1112233",
        "mobile": "+49 179 1112233",
        "email": "ursula.braun@example.de",
        "statusId": 3,
        "languageId": 1
    }'
    echo "Klient 10 erstellt: Ursula Braun"
    
    echo "Klienten erstellt: 10"
else
    echo "Klienten bereits vorhanden: $CLIENTS_COUNT"
fi

# ============================================
# GERÄTE (Devices)
# ============================================
if [ "$DEVICES_COUNT" -lt 10 ]; then
    echo ""
    echo "=== Erstelle Geräte ==="
    
    for i in {1..10}; do
        SERIAL="SN-$(printf '%06d' $i)"
        TYPE=$((($i % 3) + 1))
        post_data "/devicedetails" "{
            \"mandantId\": 1,
            \"serialNumber\": \"$SERIAL\",
            \"deviceTypeId\": $TYPE,
            \"manufacturer\": \"MedTech GmbH\",
            \"model\": \"SafeGuard Pro $i\",
            \"status\": \"Aktiv\",
            \"purchaseDate\": \"2024-0$((($i % 9) + 1))-15\",
            \"location\": \"Lager A\",
            \"notes\": \"Standardgerät Nr. $i\"
        }"
        echo "Gerät $i erstellt: $SERIAL"
    done
    
    echo "Geräte erstellt: 10"
else
    echo "Geräte bereits vorhanden: $DEVICES_COUNT"
fi

# ============================================
# DIRECT PROVIDER (Angehörige/Kontaktpersonen)
# ============================================
if [ "$DIRECT_PROVIDERS_COUNT" -lt 10 ]; then
    echo ""
    echo "=== Erstelle Direct Provider ==="
    
    NAMES=("Anna Schmidt" "Peter Müller" "Maria Weber" "Thomas Schneider" "Sabine Bauer" "Michael Koch" "Claudia Richter" "Stefan Wolf" "Monika Schäfer" "Andreas Neumann")
    RELATIONS=("Tochter" "Sohn" "Nachbar" "Enkelin" "Schwester" "Bruder" "Freundin" "Neffe" "Nichte" "Bekannter")
    
    for i in {0..9}; do
        NAME="${NAMES[$i]}"
        FIRST=$(echo $NAME | cut -d' ' -f1)
        LAST=$(echo $NAME | cut -d' ' -f2)
        REL="${RELATIONS[$i]}"
        post_data "/directProviderDetails" "{
            \"mandantId\": 1,
            \"name\": \"$NAME\",
            \"firstName\": \"$FIRST\",
            \"lastName\": \"$LAST\",
            \"street\": \"Kontaktstraße\",
            \"houseNumber\": \"$((i+1))\",
            \"zipCode\": \"1011$i\",
            \"city\": \"Berlin\",
            \"country\": \"Deutschland\",
            \"phone\": \"+49 30 555$((1000+i))\",
            \"mobile\": \"+49 170 555$((1000+i))\",
            \"email\": \"$(echo $FIRST | tr '[:upper:]' '[:lower:]').$(echo $LAST | tr '[:upper:]' '[:lower:]')@example.de\",
            \"status\": \"Aktiv\",
            \"notes\": \"$REL des Klienten\"
        }"
        echo "Direct Provider $((i+1)) erstellt: $NAME ($REL)"
    done
    
    echo "Direct Provider erstellt: 10"
else
    echo "Direct Provider bereits vorhanden: $DIRECT_PROVIDERS_COUNT"
fi

# ============================================
# PROFESSIONAL PROVIDER (Ärzte/Pflegedienste)
# ============================================
if [ "$PROF_PROVIDERS_COUNT" -lt 10 ]; then
    echo ""
    echo "=== Erstelle Professional Provider ==="
    
    DOCS=("Dr. med. Klaus Hartmann:Allgemeinmedizin" "Dr. med. Sabine Krause:Kardiologie" "Dr. med. Michael Berg:Neurologie" "Dr. med. Eva Richter:Geriatrie" "Pflegedienst Sonnenschein:Ambulante Pflege" "Dr. med. Thomas Winter:Innere Medizin" "Sanitätshaus MedCare:Hilfsmittel" "Dr. med. Andrea Sommer:Orthopädie" "Apotheke am Markt:Pharmazie" "Physiotherapie Vital:Physiotherapie")
    
    for i in {0..9}; do
        IFS=':' read -r NAME SPEC <<< "${DOCS[$i]}"
        post_data "/professionalProviderDetails" "{
            \"mandantId\": 1,
            \"name\": \"$NAME\",
            \"specialty\": \"$SPEC\",
            \"street\": \"Praxisstraße\",
            \"houseNumber\": \"$((i+1))\",
            \"zipCode\": \"1012$i\",
            \"city\": \"Berlin\",
            \"country\": \"Deutschland\",
            \"phone\": \"+49 30 666$((1000+i))\",
            \"mobile\": \"+49 171 666$((1000+i))\",
            \"email\": \"praxis$((i+1))@example.de\",
            \"status\": \"Aktiv\",
            \"licenseNumber\": \"LIC-$((100000+i))\",
            \"notes\": \"Fachgebiet: $SPEC\"
        }"
        echo "Professional Provider $((i+1)) erstellt: $NAME ($SPEC)"
    done
    
    echo "Professional Provider erstellt: 10"
else
    echo "Professional Provider bereits vorhanden: $PROF_PROVIDERS_COUNT"
fi

# ============================================
# NOTRUFGERÄTE (Emergency Devices) - Realistische Hausnotruf-Geräte
# ============================================
if [ "$EMERGENCY_DEVICES_COUNT" -lt 10 ]; then
    echo ""
    echo "=== Erstelle Notrufgeräte (Hausnotruf) ==="
    
    # Realistische Hausnotruf-Geräte von bekannten Herstellern
    # Format: "Hersteller:Modell:Typ"
    DEVICES=(
        "Tunstall:Lifeline Vi+:Basisstation"
        "Tunstall:MyAmie:Mobiler Notruf"
        "Neat:Falcon:Basisstation"
        "Neat:Atom:Funkfinger"
        "Telealarm:TA74:Basisstation"
        "Telealarm:TA35 Mobil:Mobiler Notruf"
        "Doro:Secure 580:Seniorenhandy mit Notruf"
        "Philips Lifeline:HomeSafe:Basisstation"
        "Philips Lifeline:GoSafe 2:Mobiler Notruf mit GPS"
        "Bosch:HTS 2110:Basisstation"
        "Beghelli:Salvalavita:Notrufuhr"
        "Libify:Libify Home:Smart Home Notruf"
    )
    
    for i in {0..11}; do
        IFS=':' read -r MANUF MODEL TYPE <<< "${DEVICES[$i]}"
        SERIAL="HNR-$(printf '%06d' $((i+1)))-$(date +%Y)"
        post_data "/ServiceHub/devices" "{
            \"deviceName\": \"$MODEL\",
            \"deviceType\": \"$TYPE\",
            \"manufacturer\": \"$MANUF\",
            \"model\": \"$MODEL\",
            \"serialNumber\": \"$SERIAL\",
            \"phoneNumber\": \"+49 800 555$((1000+i))\",
            \"sipIdentifier\": \"sip:hnr$((i+1))@sipgate.de\",
            \"clientId\": $(( (i % 10) + 1 ))
        }"
        echo "Notrufgerät $((i+1)) erstellt: $MANUF $MODEL ($TYPE)"
    done
    
    echo "Notrufgeräte erstellt: 12"
else
    echo "Notrufgeräte bereits vorhanden: $EMERGENCY_DEVICES_COUNT"
fi

# ============================================
# NOTFALLKONTAKTE (Emergency Contacts)
# ============================================
echo ""
echo "=== Erstelle Notfallkontakte ==="

CONTACTS=("Maria Mustermann:Ehefrau" "Thomas Mustermann:Sohn" "Lisa Musterfrau:Tochter" "Karl Meier:Sohn" "Petra Schulz:Tochter" "Frank Fischer:Nachbar" "Gisela Wagner:Schwester" "Rolf Becker:Bruder" "Heike Hoffmann:Enkelin" "Jürgen Braun:Ehemann")

for i in {0..9}; do
    IFS=':' read -r NAME REL <<< "${CONTACTS[$i]}"
    FIRST=$(echo $NAME | cut -d' ' -f1)
    LAST=$(echo $NAME | cut -d' ' -f2)
    post_data "/ServiceHub/contacts" "{
        \"clientId\": $((i+1)),
        \"firstName\": \"$FIRST\",
        \"lastName\": \"$LAST\",
        \"relationship\": \"$REL\",
        \"phoneNumber\": \"+49 30 777$((1000+i))\",
        \"mobileNumber\": \"+49 172 777$((1000+i))\",
        \"email\": \"$(echo $FIRST | tr '[:upper:]' '[:lower:]').$(echo $LAST | tr '[:upper:]' '[:lower:]')@example.de\",
        \"priority\": $((i+1)),
        \"isAvailable24h\": true,
        \"hasKey\": $([ $((i % 2)) -eq 0 ] && echo "true" || echo "false"),
        \"notes\": \"$REL - Erreichbar rund um die Uhr\"
    }"
    echo "Notfallkontakt $((i+1)) erstellt: $NAME ($REL)"
done

echo "Notfallkontakte erstellt: 10"

# ============================================
# ABSCHLUSS
# ============================================
echo ""
echo "=== Seed-Daten Zusammenfassung ==="
echo "Klienten: $(check_data_exists '/clients')"
echo "Geräte: $(check_data_exists '/devices')"
echo "Direct Provider: $(check_data_exists '/directProvider')"
echo "Professional Provider: $(check_data_exists '/professionalProvider')"
echo "Notrufgeräte: $(check_data_exists '/ServiceHub/devices')"
echo ""
echo "Seed-Daten erfolgreich geladen!"
echo "Endzeit: $(date)"
