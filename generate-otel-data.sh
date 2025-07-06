#!/bin/bash

# Script per generare dati OpenTelemetry di test
OTEL_ENDPOINT="http://localhost:4317"

echo "Generando dati OpenTelemetry di test..."

# Genera log di test
curl -X POST "$OTEL_ENDPOINT/v1/logs" \
  -H "Content-Type: application/json" \
  -d '{
    "resourceLogs": [
      {
        "resource": {
          "attributes": [
            {
              "key": "service.name",
              "value": {"stringValue": "dndtracker-test"}
            },
            {
              "key": "service.version",
              "value": {"stringValue": "1.0.0"}
            }
          ]
        },
        "scopeLogs": [
          {
            "scope": {
              "name": "test-scope"
            },
            "logRecords": [
              {
                "timeUnixNano": "'$(date +%s%N)'",
                "severityText": "INFO",
                "body": {
                  "stringValue": "Test log message for OpenTelemetry"
                },
                "attributes": [
                  {
                    "key": "test.attribute",
                    "value": {"stringValue": "test-value"}
                  }
                ]
              }
            ]
          }
        ]
      }
    ]
  }'

echo -e "\nGenerando trace di test..."

# Genera trace di test
curl -X POST "$OTEL_ENDPOINT/v1/traces" \
  -H "Content-Type: application/json" \
  -d '{
    "resourceSpans": [
      {
        "resource": {
          "attributes": [
            {
              "key": "service.name",
              "value": {"stringValue": "dndtracker-test"}
            }
          ]
        },
        "scopeSpans": [
          {
            "scope": {
              "name": "test-tracer"
            },
            "spans": [
              {
                "traceId": "'$(openssl rand -hex 16)'",
                "spanId": "'$(openssl rand -hex 8)'",
                "name": "test-span",
                "kind": 1,
                "startTimeUnixNano": "'$(date +%s%N)'",
                "endTimeUnixNano": "'$(($(date +%s%N) + 1000000000))'",
                "attributes": [
                  {
                    "key": "test.operation",
                    "value": {"stringValue": "test-operation"}
                  }
                ]
              }
            ]
          }
        ]
      }
    ]
  }'

echo -e "\nDati OpenTelemetry generati con successo!"
