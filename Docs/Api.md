# eStore API

- [eStore API](#estore-api)
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
    "email": "john.doe@gmail.com",
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
    "email": "john.doe@gmail.com",
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
    "email": "john.doe@gmail.com",
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
    "email": "john.doe@gmail.com",
    "token": "eyjhb.z9dqcnYoX"
}
```