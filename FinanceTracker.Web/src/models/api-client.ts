export class Client {
  private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
  private baseUrl: string;
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

  constructor(
    baseUrl?: string,
    http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }
  ) {
    this.http = http ? http : (window as any);
    this.baseUrl = baseUrl ?? '';
  }

  /**
   * @return Success
   */
  getAllAccountTypes(): Promise<AccountType[]> {
    let url_ = this.baseUrl + '/api/v1/accountTypes';
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAllAccountTypes(_response);
    });
  }

  protected processGetAllAccountTypes(response: Response): Promise<AccountType[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(AccountType.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<AccountType[]>(null as any);
  }

  /**
   * @return Created
   */
  createAccountType(body: AccountType): Promise<AccountType> {
    let url_ = this.baseUrl + '/api/v1/accountTypes';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(body);

    let options_: RequestInit = {
      body: content_,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processCreateAccountType(_response);
    });
  }

  protected processCreateAccountType(response: Response): Promise<AccountType> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 201) {
      return response.text().then(_responseText => {
        let result201: any = null;
        let resultData201 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result201 = AccountType.fromJS(resultData201);
        return result201;
      });
    } else if (status === 400) {
      return response.text().then(_responseText => {
        let result400: any = null;
        let resultData400 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result400 = ProblemDetails.fromJS(resultData400);
        return throwException('Bad Request', status, _responseText, _headers, result400);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<AccountType>(null as any);
  }

  /**
   * @return Success
   */
  getAllAccounts(): Promise<Account[]> {
    let url_ = this.baseUrl + '/api/v1/accounts';
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAllAccounts(_response);
    });
  }

  protected processGetAllAccounts(response: Response): Promise<Account[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(Account.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<Account[]>(null as any);
  }

  /**
   * @return Created
   */
  createAccount(body: Account): Promise<Account> {
    let url_ = this.baseUrl + '/api/v1/accounts';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(body);

    let options_: RequestInit = {
      body: content_,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processCreateAccount(_response);
    });
  }

  protected processCreateAccount(response: Response): Promise<Account> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 201) {
      return response.text().then(_responseText => {
        let result201: any = null;
        let resultData201 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result201 = Account.fromJS(resultData201);
        return result201;
      });
    } else if (status === 400) {
      return response.text().then(_responseText => {
        let result400: any = null;
        let resultData400 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result400 = ProblemDetails.fromJS(resultData400);
        return throwException('Bad Request', status, _responseText, _headers, result400);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<Account>(null as any);
  }

  /**
   * @return Success
   */
  getAccountById(id: number): Promise<Account> {
    let url_ = this.baseUrl + '/api/v1/accounts/{id}';
    if (id === undefined || id === null) throw new Error("The parameter 'id' must be defined.");
    url_ = url_.replace('{id}', encodeURIComponent('' + id));
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAccountById(_response);
    });
  }

  protected processGetAccountById(response: Response): Promise<Account> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result200 = Account.fromJS(resultData200);
        return result200;
      });
    } else if (status === 404) {
      return response.text().then(_responseText => {
        let result404: any = null;
        let resultData404 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result404 = ProblemDetails.fromJS(resultData404);
        return throwException('Not Found', status, _responseText, _headers, result404);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<Account>(null as any);
  }

  /**
   * @return Success
   */
  getAccountTransactions(id: number): Promise<Transaction[]> {
    let url_ = this.baseUrl + '/api/v1/accounts/{id}/transactions';
    if (id === undefined || id === null) throw new Error("The parameter 'id' must be defined.");
    url_ = url_.replace('{id}', encodeURIComponent('' + id));
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAccountTransactions(_response);
    });
  }

  protected processGetAccountTransactions(response: Response): Promise<Transaction[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(Transaction.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status === 404) {
      return response.text().then(_responseText => {
        let result404: any = null;
        let resultData404 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result404 = ProblemDetails.fromJS(resultData404);
        return throwException('Not Found', status, _responseText, _headers, result404);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<Transaction[]>(null as any);
  }

  /**
   * @return Success
   */
  getAccountRecurringTransactions(id: number): Promise<RecurringTransaction[]> {
    let url_ = this.baseUrl + '/api/v1/accounts/{id}/recurringTransactions';
    if (id === undefined || id === null) throw new Error("The parameter 'id' must be defined.");
    url_ = url_.replace('{id}', encodeURIComponent('' + id));
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAccountRecurringTransactions(_response);
    });
  }

  protected processGetAccountRecurringTransactions(
    response: Response
  ): Promise<RecurringTransaction[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(RecurringTransaction.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status === 404) {
      return response.text().then(_responseText => {
        let result404: any = null;
        let resultData404 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result404 = ProblemDetails.fromJS(resultData404);
        return throwException('Not Found', status, _responseText, _headers, result404);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<RecurringTransaction[]>(null as any);
  }

  /**
   * @return Success
   */
  getAllRecurringTransactions(): Promise<RecurringTransaction[]> {
    let url_ = this.baseUrl + '/api/v1/recurringTransactions';
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAllRecurringTransactions(_response);
    });
  }

  protected processGetAllRecurringTransactions(
    response: Response
  ): Promise<RecurringTransaction[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(RecurringTransaction.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<RecurringTransaction[]>(null as any);
  }

  /**
   * @return Created
   */
  createRecurringTransaction(body: RecurringTransaction): Promise<RecurringTransaction> {
    let url_ = this.baseUrl + '/api/v1/recurringTransactions';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(body);

    let options_: RequestInit = {
      body: content_,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processCreateRecurringTransaction(_response);
    });
  }

  protected processCreateRecurringTransaction(response: Response): Promise<RecurringTransaction> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 201) {
      return response.text().then(_responseText => {
        let result201: any = null;
        let resultData201 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result201 = RecurringTransaction.fromJS(resultData201);
        return result201;
      });
    } else if (status === 400) {
      return response.text().then(_responseText => {
        let result400: any = null;
        let resultData400 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result400 = ProblemDetails.fromJS(resultData400);
        return throwException('Bad Request', status, _responseText, _headers, result400);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<RecurringTransaction>(null as any);
  }

  /**
   * @return Success
   */
  getAllTransactionCategories(): Promise<TransactionCategory[]> {
    let url_ = this.baseUrl + '/api/v1/transactionCategories';
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAllTransactionCategories(_response);
    });
  }

  protected processGetAllTransactionCategories(response: Response): Promise<TransactionCategory[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(TransactionCategory.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<TransactionCategory[]>(null as any);
  }

  /**
   * @return Created
   */
  createTransactionCategory(body: TransactionCategory): Promise<TransactionCategory> {
    let url_ = this.baseUrl + '/api/v1/transactionCategories';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(body);

    let options_: RequestInit = {
      body: content_,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processCreateTransactionCategory(_response);
    });
  }

  protected processCreateTransactionCategory(response: Response): Promise<TransactionCategory> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 201) {
      return response.text().then(_responseText => {
        let result201: any = null;
        let resultData201 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result201 = TransactionCategory.fromJS(resultData201);
        return result201;
      });
    } else if (status === 400) {
      return response.text().then(_responseText => {
        let result400: any = null;
        let resultData400 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result400 = ProblemDetails.fromJS(resultData400);
        return throwException('Bad Request', status, _responseText, _headers, result400);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<TransactionCategory>(null as any);
  }

  /**
   * @return Success
   */
  getAllTransactionSplits(): Promise<TransactionSplit[]> {
    let url_ = this.baseUrl + '/api/v1/transactionSplits';
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAllTransactionSplits(_response);
    });
  }

  protected processGetAllTransactionSplits(response: Response): Promise<TransactionSplit[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(TransactionSplit.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<TransactionSplit[]>(null as any);
  }

  /**
   * @return Created
   */
  createTransactionSplit(body: TransactionSplit): Promise<TransactionSplit> {
    let url_ = this.baseUrl + '/api/v1/transactionSplits';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(body);

    let options_: RequestInit = {
      body: content_,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processCreateTransactionSplit(_response);
    });
  }

  protected processCreateTransactionSplit(response: Response): Promise<TransactionSplit> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 201) {
      return response.text().then(_responseText => {
        let result201: any = null;
        let resultData201 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result201 = TransactionSplit.fromJS(resultData201);
        return result201;
      });
    } else if (status === 400) {
      return response.text().then(_responseText => {
        let result400: any = null;
        let resultData400 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result400 = ProblemDetails.fromJS(resultData400);
        return throwException('Bad Request', status, _responseText, _headers, result400);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<TransactionSplit>(null as any);
  }

  /**
   * @return Success
   */
  getAllTransactionTypes(): Promise<TransactionType[]> {
    let url_ = this.baseUrl + '/api/v1/transactionTypes';
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAllTransactionTypes(_response);
    });
  }

  protected processGetAllTransactionTypes(response: Response): Promise<TransactionType[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(TransactionType.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<TransactionType[]>(null as any);
  }

  /**
   * @return Created
   */
  createTransactionType(body: TransactionType): Promise<TransactionType> {
    let url_ = this.baseUrl + '/api/v1/transactionTypes';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(body);

    let options_: RequestInit = {
      body: content_,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processCreateTransactionType(_response);
    });
  }

  protected processCreateTransactionType(response: Response): Promise<TransactionType> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 201) {
      return response.text().then(_responseText => {
        let result201: any = null;
        let resultData201 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result201 = TransactionType.fromJS(resultData201);
        return result201;
      });
    } else if (status === 400) {
      return response.text().then(_responseText => {
        let result400: any = null;
        let resultData400 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result400 = ProblemDetails.fromJS(resultData400);
        return throwException('Bad Request', status, _responseText, _headers, result400);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<TransactionType>(null as any);
  }

  /**
   * @return Success
   */
  getAllTransactions(): Promise<Transaction[]> {
    let url_ = this.baseUrl + '/api/v1/transactions';
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetAllTransactions(_response);
    });
  }

  protected processGetAllTransactions(response: Response): Promise<Transaction[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(Transaction.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<Transaction[]>(null as any);
  }

  /**
   * @return Created
   */
  createTransaction(body: Transaction): Promise<Transaction> {
    let url_ = this.baseUrl + '/api/v1/transactions';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = JSON.stringify(body);

    let options_: RequestInit = {
      body: content_,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processCreateTransaction(_response);
    });
  }

  protected processCreateTransaction(response: Response): Promise<Transaction> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 201) {
      return response.text().then(_responseText => {
        let result201: any = null;
        let resultData201 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result201 = Transaction.fromJS(resultData201);
        return result201;
      });
    } else if (status === 400) {
      return response.text().then(_responseText => {
        let result400: any = null;
        let resultData400 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result400 = ProblemDetails.fromJS(resultData400);
        return throwException('Bad Request', status, _responseText, _headers, result400);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<Transaction>(null as any);
  }

  /**
   * @return Success
   */
  getTransactionById(id: number): Promise<Transaction> {
    let url_ = this.baseUrl + '/api/v1/transactions/{id}';
    if (id === undefined || id === null) throw new Error("The parameter 'id' must be defined.");
    url_ = url_.replace('{id}', encodeURIComponent('' + id));
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetTransactionById(_response);
    });
  }

  protected processGetTransactionById(response: Response): Promise<Transaction> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result200 = Transaction.fromJS(resultData200);
        return result200;
      });
    } else if (status === 404) {
      return response.text().then(_responseText => {
        let result404: any = null;
        let resultData404 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result404 = ProblemDetails.fromJS(resultData404);
        return throwException('Not Found', status, _responseText, _headers, result404);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<Transaction>(null as any);
  }

  /**
   * @return Success
   */
  getTransactionSplits(id: number): Promise<TransactionSplit[]> {
    let url_ = this.baseUrl + '/api/v1/transactions/{id}/transactionSplits';
    if (id === undefined || id === null) throw new Error("The parameter 'id' must be defined.");
    url_ = url_.replace('{id}', encodeURIComponent('' + id));
    url_ = url_.replace(/[?&]$/, '');

    let options_: RequestInit = {
      method: 'GET',
      headers: {
        Accept: 'application/json',
      },
    };

    return this.http.fetch(url_, options_).then((_response: Response) => {
      return this.processGetTransactionSplits(_response);
    });
  }

  protected processGetTransactionSplits(response: Response): Promise<TransactionSplit[]> {
    const status = response.status;
    let _headers: any = {};
    if (response.headers && response.headers.forEach) {
      response.headers.forEach((v: any, k: any) => (_headers[k] = v));
    }
    if (status === 200) {
      return response.text().then(_responseText => {
        let result200: any = null;
        let resultData200 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        if (Array.isArray(resultData200)) {
          result200 = [] as any;
          for (let item of resultData200) result200!.push(TransactionSplit.fromJS(item));
        } else {
          result200 = <any>null;
        }
        return result200;
      });
    } else if (status === 404) {
      return response.text().then(_responseText => {
        let result404: any = null;
        let resultData404 =
          _responseText === '' ? null : JSON.parse(_responseText, this.jsonParseReviver);
        result404 = ProblemDetails.fromJS(resultData404);
        return throwException('Not Found', status, _responseText, _headers, result404);
      });
    } else if (status !== 200 && status !== 204) {
      return response.text().then(_responseText => {
        return throwException(
          'An unexpected server error occurred.',
          status,
          _responseText,
          _headers
        );
      });
    }
    return Promise.resolve<TransactionSplit[]>(null as any);
  }
}

export class Account implements IAccount {
  id?: number;
  name?: string | undefined;
  openingBalance?: number;
  openDate?: Date;
  accountTypeId?: number;
  accountType?: AccountType;
  transactions?: Transaction[] | undefined;
  recurringTransactions?: RecurringTransaction[] | undefined;

  constructor(data?: IAccount) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.id = _data['id'];
      this.name = _data['name'];
      this.openingBalance = _data['openingBalance'];
      this.openDate = _data['openDate'] ? new Date(_data['openDate'].toString()) : <any>undefined;
      this.accountTypeId = _data['accountTypeId'];
      this.accountType = _data['accountType']
        ? AccountType.fromJS(_data['accountType'])
        : <any>undefined;
      if (Array.isArray(_data['transactions'])) {
        this.transactions = [] as any;
        for (let item of _data['transactions']) this.transactions!.push(Transaction.fromJS(item));
      }
      if (Array.isArray(_data['recurringTransactions'])) {
        this.recurringTransactions = [] as any;
        for (let item of _data['recurringTransactions'])
          this.recurringTransactions!.push(RecurringTransaction.fromJS(item));
      }
    }
  }

  static fromJS(data: any): Account {
    data = typeof data === 'object' ? data : {};
    let result = new Account();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['id'] = this.id;
    data['name'] = this.name;
    data['openingBalance'] = this.openingBalance;
    data['openDate'] = this.openDate ? formatDate(this.openDate) : <any>undefined;
    data['accountTypeId'] = this.accountTypeId;
    data['accountType'] = this.accountType ? this.accountType.toJSON() : <any>undefined;
    if (Array.isArray(this.transactions)) {
      data['transactions'] = [];
      for (let item of this.transactions) data['transactions'].push(item.toJSON());
    }
    if (Array.isArray(this.recurringTransactions)) {
      data['recurringTransactions'] = [];
      for (let item of this.recurringTransactions)
        data['recurringTransactions'].push(item.toJSON());
    }
    return data;
  }
}

export interface IAccount {
  id?: number;
  name?: string | undefined;
  openingBalance?: number;
  openDate?: Date;
  accountTypeId?: number;
  accountType?: AccountType;
  transactions?: Transaction[] | undefined;
  recurringTransactions?: RecurringTransaction[] | undefined;
}

export class AccountType implements IAccountType {
  id?: number;
  type?: string | undefined;
  accounts?: Account[] | undefined;

  constructor(data?: IAccountType) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.id = _data['id'];
      this.type = _data['type'];
      if (Array.isArray(_data['accounts'])) {
        this.accounts = [] as any;
        for (let item of _data['accounts']) this.accounts!.push(Account.fromJS(item));
      }
    }
  }

  static fromJS(data: any): AccountType {
    data = typeof data === 'object' ? data : {};
    let result = new AccountType();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['id'] = this.id;
    data['type'] = this.type;
    if (Array.isArray(this.accounts)) {
      data['accounts'] = [];
      for (let item of this.accounts) data['accounts'].push(item.toJSON());
    }
    return data;
  }
}

export interface IAccountType {
  id?: number;
  type?: string | undefined;
  accounts?: Account[] | undefined;
}

export class ProblemDetails implements IProblemDetails {
  type?: string | undefined;
  title?: string | undefined;
  status?: number | undefined;
  detail?: string | undefined;
  instance?: string | undefined;

  [key: string]: any;

  constructor(data?: IProblemDetails) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      for (var property in _data) {
        if (_data.hasOwnProperty(property)) this[property] = _data[property];
      }
      this.type = _data['type'];
      this.title = _data['title'];
      this.status = _data['status'];
      this.detail = _data['detail'];
      this.instance = _data['instance'];
    }
  }

  static fromJS(data: any): ProblemDetails {
    data = typeof data === 'object' ? data : {};
    let result = new ProblemDetails();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    for (var property in this) {
      if (this.hasOwnProperty(property)) data[property] = this[property];
    }
    data['type'] = this.type;
    data['title'] = this.title;
    data['status'] = this.status;
    data['detail'] = this.detail;
    data['instance'] = this.instance;
    return data;
  }
}

export interface IProblemDetails {
  type?: string | undefined;
  title?: string | undefined;
  status?: number | undefined;
  detail?: string | undefined;
  instance?: string | undefined;

  [key: string]: any;
}

export class RecurringTransaction implements IRecurringTransaction {
  id?: number;
  startDate?: Date;
  endDate?: Date | undefined;
  amount?: number;
  amountVariancePercentage?: number;
  description?: string | undefined;
  recurrenceCronExpression?: string | undefined;
  accountId?: number;
  transactionTypeId?: number;
  categoryId?: number;
  account?: Account;
  category?: TransactionCategory;
  transactionType?: TransactionType;

  constructor(data?: IRecurringTransaction) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.id = _data['id'];
      this.startDate = _data['startDate']
        ? new Date(_data['startDate'].toString())
        : <any>undefined;
      this.endDate = _data['endDate'] ? new Date(_data['endDate'].toString()) : <any>undefined;
      this.amount = _data['amount'];
      this.amountVariancePercentage = _data['amountVariancePercentage'];
      this.description = _data['description'];
      this.recurrenceCronExpression = _data['recurrenceCronExpression'];
      this.accountId = _data['accountId'];
      this.transactionTypeId = _data['transactionTypeId'];
      this.categoryId = _data['categoryId'];
      this.account = _data['account'] ? Account.fromJS(_data['account']) : <any>undefined;
      this.category = _data['category']
        ? TransactionCategory.fromJS(_data['category'])
        : <any>undefined;
      this.transactionType = _data['transactionType']
        ? TransactionType.fromJS(_data['transactionType'])
        : <any>undefined;
    }
  }

  static fromJS(data: any): RecurringTransaction {
    data = typeof data === 'object' ? data : {};
    let result = new RecurringTransaction();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['id'] = this.id;
    data['startDate'] = this.startDate ? formatDate(this.startDate) : <any>undefined;
    data['endDate'] = this.endDate ? formatDate(this.endDate) : <any>undefined;
    data['amount'] = this.amount;
    data['amountVariancePercentage'] = this.amountVariancePercentage;
    data['description'] = this.description;
    data['recurrenceCronExpression'] = this.recurrenceCronExpression;
    data['accountId'] = this.accountId;
    data['transactionTypeId'] = this.transactionTypeId;
    data['categoryId'] = this.categoryId;
    data['account'] = this.account ? this.account.toJSON() : <any>undefined;
    data['category'] = this.category ? this.category.toJSON() : <any>undefined;
    data['transactionType'] = this.transactionType ? this.transactionType.toJSON() : <any>undefined;
    return data;
  }
}

export interface IRecurringTransaction {
  id?: number;
  startDate?: Date;
  endDate?: Date | undefined;
  amount?: number;
  amountVariancePercentage?: number;
  description?: string | undefined;
  recurrenceCronExpression?: string | undefined;
  accountId?: number;
  transactionTypeId?: number;
  categoryId?: number;
  account?: Account;
  category?: TransactionCategory;
  transactionType?: TransactionType;
}

export class Transaction implements ITransaction {
  id?: number;
  transactionDate?: Date;
  amount?: number;
  description?: string | undefined;
  accountId?: number;
  transactionTypeId?: number;
  categoryId?: number;
  account?: Account;
  category?: TransactionCategory;
  transactionSplits?: TransactionSplit[] | undefined;
  transactionType?: TransactionType;

  constructor(data?: ITransaction) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.id = _data['id'];
      this.transactionDate = _data['transactionDate']
        ? new Date(_data['transactionDate'].toString())
        : <any>undefined;
      this.amount = _data['amount'];
      this.description = _data['description'];
      this.accountId = _data['accountId'];
      this.transactionTypeId = _data['transactionTypeId'];
      this.categoryId = _data['categoryId'];
      this.account = _data['account'] ? Account.fromJS(_data['account']) : <any>undefined;
      this.category = _data['category']
        ? TransactionCategory.fromJS(_data['category'])
        : <any>undefined;
      if (Array.isArray(_data['transactionSplits'])) {
        this.transactionSplits = [] as any;
        for (let item of _data['transactionSplits'])
          this.transactionSplits!.push(TransactionSplit.fromJS(item));
      }
      this.transactionType = _data['transactionType']
        ? TransactionType.fromJS(_data['transactionType'])
        : <any>undefined;
    }
  }

  static fromJS(data: any): Transaction {
    data = typeof data === 'object' ? data : {};
    let result = new Transaction();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['id'] = this.id;
    data['transactionDate'] = this.transactionDate
      ? formatDate(this.transactionDate)
      : <any>undefined;
    data['amount'] = this.amount;
    data['description'] = this.description;
    data['accountId'] = this.accountId;
    data['transactionTypeId'] = this.transactionTypeId;
    data['categoryId'] = this.categoryId;
    data['account'] = this.account ? this.account.toJSON() : <any>undefined;
    data['category'] = this.category ? this.category.toJSON() : <any>undefined;
    if (Array.isArray(this.transactionSplits)) {
      data['transactionSplits'] = [];
      for (let item of this.transactionSplits) data['transactionSplits'].push(item.toJSON());
    }
    data['transactionType'] = this.transactionType ? this.transactionType.toJSON() : <any>undefined;
    return data;
  }
}

export interface ITransaction {
  id?: number;
  transactionDate?: Date;
  amount?: number;
  description?: string | undefined;
  accountId?: number;
  transactionTypeId?: number;
  categoryId?: number;
  account?: Account;
  category?: TransactionCategory;
  transactionSplits?: TransactionSplit[] | undefined;
  transactionType?: TransactionType;
}

export class TransactionCategory implements ITransactionCategory {
  id?: number;
  category?: string | undefined;
  description?: string | undefined;
  transactionSplits?: TransactionSplit[] | undefined;
  transactions?: Transaction[] | undefined;
  recurringTransactions?: RecurringTransaction[] | undefined;

  constructor(data?: ITransactionCategory) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.id = _data['id'];
      this.category = _data['category'];
      this.description = _data['description'];
      if (Array.isArray(_data['transactionSplits'])) {
        this.transactionSplits = [] as any;
        for (let item of _data['transactionSplits'])
          this.transactionSplits!.push(TransactionSplit.fromJS(item));
      }
      if (Array.isArray(_data['transactions'])) {
        this.transactions = [] as any;
        for (let item of _data['transactions']) this.transactions!.push(Transaction.fromJS(item));
      }
      if (Array.isArray(_data['recurringTransactions'])) {
        this.recurringTransactions = [] as any;
        for (let item of _data['recurringTransactions'])
          this.recurringTransactions!.push(RecurringTransaction.fromJS(item));
      }
    }
  }

  static fromJS(data: any): TransactionCategory {
    data = typeof data === 'object' ? data : {};
    let result = new TransactionCategory();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['id'] = this.id;
    data['category'] = this.category;
    data['description'] = this.description;
    if (Array.isArray(this.transactionSplits)) {
      data['transactionSplits'] = [];
      for (let item of this.transactionSplits) data['transactionSplits'].push(item.toJSON());
    }
    if (Array.isArray(this.transactions)) {
      data['transactions'] = [];
      for (let item of this.transactions) data['transactions'].push(item.toJSON());
    }
    if (Array.isArray(this.recurringTransactions)) {
      data['recurringTransactions'] = [];
      for (let item of this.recurringTransactions)
        data['recurringTransactions'].push(item.toJSON());
    }
    return data;
  }
}

export interface ITransactionCategory {
  id?: number;
  category?: string | undefined;
  description?: string | undefined;
  transactionSplits?: TransactionSplit[] | undefined;
  transactions?: Transaction[] | undefined;
  recurringTransactions?: RecurringTransaction[] | undefined;
}

export class TransactionSplit implements ITransactionSplit {
  id?: number;
  transactionId?: number;
  amount?: number;
  description?: string | undefined;
  categoryId?: number;
  category?: TransactionCategory;
  transaction?: Transaction;

  constructor(data?: ITransactionSplit) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.id = _data['id'];
      this.transactionId = _data['transactionId'];
      this.amount = _data['amount'];
      this.description = _data['description'];
      this.categoryId = _data['categoryId'];
      this.category = _data['category']
        ? TransactionCategory.fromJS(_data['category'])
        : <any>undefined;
      this.transaction = _data['transaction']
        ? Transaction.fromJS(_data['transaction'])
        : <any>undefined;
    }
  }

  static fromJS(data: any): TransactionSplit {
    data = typeof data === 'object' ? data : {};
    let result = new TransactionSplit();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['id'] = this.id;
    data['transactionId'] = this.transactionId;
    data['amount'] = this.amount;
    data['description'] = this.description;
    data['categoryId'] = this.categoryId;
    data['category'] = this.category ? this.category.toJSON() : <any>undefined;
    data['transaction'] = this.transaction ? this.transaction.toJSON() : <any>undefined;
    return data;
  }
}

export interface ITransactionSplit {
  id?: number;
  transactionId?: number;
  amount?: number;
  description?: string | undefined;
  categoryId?: number;
  category?: TransactionCategory;
  transaction?: Transaction;
}

export class TransactionType implements ITransactionType {
  id?: number;
  type?: string | undefined;
  transactions?: Transaction[] | undefined;
  recurringTransactions?: RecurringTransaction[] | undefined;

  constructor(data?: ITransactionType) {
    if (data) {
      for (var property in data) {
        if (data.hasOwnProperty(property)) (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  init(_data?: any) {
    if (_data) {
      this.id = _data['id'];
      this.type = _data['type'];
      if (Array.isArray(_data['transactions'])) {
        this.transactions = [] as any;
        for (let item of _data['transactions']) this.transactions!.push(Transaction.fromJS(item));
      }
      if (Array.isArray(_data['recurringTransactions'])) {
        this.recurringTransactions = [] as any;
        for (let item of _data['recurringTransactions'])
          this.recurringTransactions!.push(RecurringTransaction.fromJS(item));
      }
    }
  }

  static fromJS(data: any): TransactionType {
    data = typeof data === 'object' ? data : {};
    let result = new TransactionType();
    result.init(data);
    return result;
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data['id'] = this.id;
    data['type'] = this.type;
    if (Array.isArray(this.transactions)) {
      data['transactions'] = [];
      for (let item of this.transactions) data['transactions'].push(item.toJSON());
    }
    if (Array.isArray(this.recurringTransactions)) {
      data['recurringTransactions'] = [];
      for (let item of this.recurringTransactions)
        data['recurringTransactions'].push(item.toJSON());
    }
    return data;
  }
}

export interface ITransactionType {
  id?: number;
  type?: string | undefined;
  transactions?: Transaction[] | undefined;
  recurringTransactions?: RecurringTransaction[] | undefined;
}

function formatDate(d: Date) {
  return (
    d.getFullYear() +
    '-' +
    (d.getMonth() < 9 ? '0' + (d.getMonth() + 1) : d.getMonth() + 1) +
    '-' +
    (d.getDate() < 10 ? '0' + d.getDate() : d.getDate())
  );
}

export class ApiException extends Error {
  message: string;
  status: number;
  response: string;
  headers: { [key: string]: any };
  result: any;

  constructor(
    message: string,
    status: number,
    response: string,
    headers: { [key: string]: any },
    result: any
  ) {
    super();

    this.message = message;
    this.status = status;
    this.response = response;
    this.headers = headers;
    this.result = result;
  }

  protected isApiException = true;

  static isApiException(obj: any): obj is ApiException {
    return obj.isApiException === true;
  }
}

function throwException(
  message: string,
  status: number,
  response: string,
  headers: { [key: string]: any },
  result?: any
): any {
  if (result !== null && result !== undefined) throw result;
  else throw new ApiException(message, status, response, headers, null);
}
