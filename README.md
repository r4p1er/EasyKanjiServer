# EasyKanjiServer
The website which purpose is to help people to learn japanese kanji


## EasyKanjiServer API

### Tokens

|  API             | Description      | Authorize | Request Params | Request Body          | Response Body              |
| :--------------: | :--------------: | :-------: | :------------: | :-------------------: | :------------------------: |
| POST /api/tokens | Get JWT for auth |  None     | None           | Username and password | JWT, role and id of a user |

### Users

|  API                          | Description                     | Authorize | Request Params | Request Body                      | Response Body              |
| :---------------------------: | :-----------------------------: | :-------: | :------------: | :-------------------------------: | :------------------------: |
| GET /api/users                | Get all users                   |  Admin    | None           | None                              | Array of users             |
| GET /api/users/{id}           | Get user by id                  | Admin     | None           | None                              | User                       |
| GET /api/users/me             | Get current user                | Any       | None           | None                              | User                       |
| POST /api/users               | Create new user                 | None      | None           | Username and password             | User                       |
| PATCH /api/users/{id}         | Change user's credentials       | Any       | None           | Changes: username, password, role | No content                 |
| PATCH /api/users/kanjis/add    | Add kanjis to current user      | Any       | None           | Array of id of kanjis             | No content                 |
| PATCH /api/users/kanjis/remove | Remove kanjis from current user | Any       | None           | Array of id of kanjis             | No content                 |
| DELETE /api/users/{id}        | Delete user by id               | Any       | None           | None                              | No content                 |

### Kanjis

|  API                          | Description                       | Authorize | Request Params | Request Body                      | Response Body              |
| :---------------------------: | :-------------------------------: | :-------: | :------------: | :-------------------------------: | :------------------------: |
| GET /api/kanjis/{id}          | Get kanji by id                                                |  None     | None           | None                              | Kanji                      |
| GET /api/kanjis               | Get array of kanjis by ids                                     | None      | Array of ids (e.g. ?ids=1&ids=2&ids=3 etc.)   | None                              | Array of kanjis            |
| GET /api/kanjis/{listName}    | Get slice of kanjis from a specified list: popular or favorite | None       | Two integers: start id and end id (e.g. ?s=5&e=10)           | None                              | Array of kanjis                       |
| GET /api/kanjis/search        | Get array of kanjis by search string | None      | Search string: writing, readings, meanings, etc. (e.g. ?q=人) | None | Array of kanjis  |
| POST /api/kanjis              | Create new kanji                | Admin     | None           | Kanji object: writing, on reading, kun reading, meaning | Kanji                 |
| PATCH /api/kanjis/{id}        | Modify existing kanji      | Admin       | None           | Changes: writing, readings, meanings | No content                 |
| DELETE /api/kanjis/{id}       | Delete kanji by id               | Admin       | None           | None                              | No content                 |
