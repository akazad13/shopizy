# Shopizy API

- [Shopizy API](#Shopizy-api)
  - [Auth](#auth)
    - [Register](#register)
      - [Register Request](#register-request)
      - [Register Response](#register-response)
    - [Login](#login)
      - [Login Request](#login-request)
      - [Login Response](#login-response)

## Auth

### Register

```js
POST {{host}}/auth/register
```

#### Register Request

```json
{
    "firstName": "John",
    "LastName": "Doe",
    "phone": "+3584573969860",
    "password": "John123!"
}
```

#### Register Response

```js
200 OK
```

```json
{
    "id": "",
    "firstName": "John",
    "LastName": "Doe",
    "phone": "+3584573969860",
    "token": "eyjhb.z9dqcnYoX"
}
```

### Login

```js
POST {{host}}/auth/login
```

#### Login Request

```json
{
    "phone": "+3584573969860",
    "password": "John123!"
}
```

#### Login Response

```js
200 OK
```

```json
{
    "id": "",
    "firstName": "John",
    "LastName": "Doe",
    "phone": "+3584573969860",
    "token": "eyjhb.z9dqcnYoX"
}
```