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
| PATCH /api/users/kanji/add    | Add kanjis to current user      | Any       | None           | Array of id of kanjis             | No content                 |
| PATCH /api/users/kanji/remove | Remove kanjis from current user | Any       | None           | Array of id of kanjis             | No content                 |
| DELETE /api/users/{id}        | Delete user by id               | Any       | None           | None                              | No content                 |
