# Meter Read API
The Meter Read API is a simple example API that takes meter readings in CSV format, stores them in a database and allows the readings to be retrieved.

## Usage
Simply run the MeterReadApi project in a .NET Core runtime.

Swagger will be available under `/swagger`.

## Endpoints
The root endpoint is available under `/MeterRead` with the following API requests.

### seed-database
The database must be seeded with accounts before any meter readings can be uploaded. 

**NOTE: The database must be seeded on every restart, as the current implementation uses an in-memory database (see [Limitations](#limitations)**).

The `seed-database` request takes a single CSV file as form-data via the `file` key.

The CSV file must contain the following header:
```csv
AccountId,FirstName,LastName
```

Subsequent lines should then contain the account information. For example:
```csv
AccountId,FirstName,LastName
2344,Tommy,Test
2233,Barry,Test
```

**Usage**
```
POST /MeterRead/seed-database
```

**Returns**

200 OK if the database has been successfully seeded.

### meter-reading-uploads
Meter readings can be uploaded via the `meter-reading-uploads` endpoint. The request takes a single CSV file as form-data via the `file` key.

The CSV file must contain the following header:
```csv
AccountId,MeterReadingDateTime,MeterReadValue,
```

Subsequent lines should then contain meter readings. For example:
```csv
AccountId,MeterReadingDateTime,MeterReadValue,
2344,22/04/2019 09:24,1002,
2233,22/04/2019 12:25,323,
```

**NOTE: Date/times are parsed using the en-GB locale and must be in the format `dd/MM/yyyy HH:mm`**

**Usage**
```
POST /MeterRead/meter-reading-uploads
```

**Returns**

Returns the number of successful and failed uploads.
```json
{
    "successful": 28,
    "failed": 7
}
```

### get-accounts
Retrieves accounts stored in the database. There are no parameters.

**Usage**
```
GET /MeterRead/get-accounts
```

**Returns**
```json
[
    {
        "accountId": 2344,
        "firstName": "Tommy",
        "lastName": "Test",
        "meterReadings": []
    }
]
```

###  get-readings
Retrieves readings assocated with the provided account.

**Usage**
```
GET /MeterRead/get-readings?accountId=2344
```

Returns:
```json
[
    {
        "meterReadingId": 1,
        "accountId": 2344,
        "meterReadingDateTime": "2019-04-22T09:24:00+01:00",
        "meterReadValue": 1002
    },
    {
        "meterReadingId": 4,
        "accountId": 2344,
        "meterReadingDateTime": "2019-04-22T12:25:00+01:00",
        "meterReadValue": 1002
    },
    {
        "meterReadingId": 15,
        "accountId": 2344,
        "meterReadingDateTime": "2019-05-08T09:24:00+01:00",
        "meterReadValue": 0
    }
]
```

## Limitations
- The database is currently hard-coded to use an in-memory instance. Changes will be lost when the service is restarted
- The date-parsing locale is hardcoded to en-GB
- Date/times are converted into local time according to the en-GB locale. This may or may not be desirable if the original timestamps are in UTC.

## Future Enhancements
- Allow configuring the database connection string so that data may be persisted
- Add logging
- Automatically seed the database from a configured seed file
- Use UTC timestamps if readings are in UTC (requirement needs clarification)
