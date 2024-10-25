///<reference path="../../libs/jquery-ui-dist/jquery-ui.min.js" />
///<reference path="../../libs/jquery/jquery.min.js" />
(function (BootstrapData) {
    BootstrapData.dataSource = function (options) {
        this.Data = [];
        this.schema = options.schema;
        this.defaultorderbyQueryOption = !!options.queryOptions && !!options.queryOptions.$orderby ? options.queryOptions.$orderby : null
        var that = this;
        var asynchronous = options.async === undefined || options.asyncs === null ? true : options.async;
        this.buildNewRowData = function (r) {
            if (!r) {
                r = {};
            }
            r.set = function (fn, fv) {
                r[fn] = fv;
                !!that.onValueChanged && that.onValueChanged(r, fn, fv);
            };
            return r;
        };
        this.fetch = function (callback, sort, paging) {
            if (!!this.displayLoading)
                this.displayLoading();
            switch (options.type) {
                case "odata":
                    var u = options.url;
                    if (!!sort) {
                        options.queryOptions = options.queryOptions || {};
                        options.queryOptions.$orderby = sort.field + " " + sort.type;
                    } else {
                        if (!!that.defaultorderbyQueryOption)
                            options.queryOptions.$orderby = that.defaultorderbyQueryOption;
                        else {
                            !!options.queryOptions && !!options.queryOptions.$orderby && delete options.queryOptions.$orderby;
                        }
                    }

                    if (!!paging) {
                        paging.currentPage = paging.currentPage || 1;
                        options.queryOptions = options.queryOptions || {};
                        options.queryOptions.$top = paging.size;
                        if (paging.currentPage > 1) {
                            options.queryOptions.$skip = paging.size * (paging.currentPage - 1);
                        } else {
                            !!options.queryOptions.$skip && delete options.queryOptions.$skip;
                        }

                    }

                    if (!!options.queryOptions) {
                        var idx = 0;
                        for (var qo in options.queryOptions) {
                            var del = idx === 0 ? '?' : '&';
                            u = u + del + qo + '=' + options.queryOptions[qo];
                            idx++;
                        }

                    }



                    $.ajax({
                        type: 'GET',
                        url: u,
                        dataType: 'json',
                        async: asynchronous,
                        contentType: "application/json",
                        beforeSend: function (xhr) {
                            if (!!options.ajax) {
                                if (!!options.ajax.beforeSend) {
                                    options.ajax.beforeSend(xhr);
                                }
                            }
                        },
                        success: function (resp) {

                            that.Count = !!resp["@odata.count"] ? resp["@odata.count"]: null;
                            that.Data = resp.value.map(
                                function (d) {
                                    d.set = function (fn, fv) {
                                        d[fn] = fv;
                                        !!that.onValueChanged && that.onValueChanged(d, fn, fv);
                                    }
                                    return d;
                                }
                            );
                            callback(that.Data, that.Count);
                            if (!!that.hideLoading)
                                that.hideLoading();
                        },
                        error: function (err) {
                            console.error(err);
                        }
                    });

                    break;
                default:
                    this.Data = options.loadData();
                    callback(this.Data);
                    if (!!this.hideLoading)
                        this.hideLoading();
                    break;
            }

        };

        this.findRowFromCacheByKey=function(key){

            return that.Data.find((d) => d[that.schema.key] === key);
        }
       
       
       
       
       
        




    };
}(window.BootstrapData = window.BootstrapData || {}));

(function ($) {

    $.fn.customBootstrapTable = function (config) {
        function uuid() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }
        var table_uid = uuid();
        var loadingUid = uuid();
        const showLoading = (display) => {
            var loadingDiv = $(`div[uid='${loadingUid}']`);
            if (display) {
                if (loadingDiv.length === 0) {
                    $(that).append(`<div uid="${loadingUid}" class="overlay"><div style="position: relative;left: 50%;top:50%;"  class="spinner-border text-dark m-1" role="status">
                        <span class="sr-only">Loading...</span>
                    </div></div>`);
                } else {
                    loadingDiv.show();
                }

            } else {
                if (loadingDiv.length > 0) {
                    loadingDiv.hide();
                }

            }

        };
        var sortIcons = {
            asc: 'sorting_asc',
            desc: 'sorting_desc',
            default: 'sorting'
        };
        var that = this;
        $(that).empty();
        showLoading(true);
        var datasource = config.datasource;
        var columns = config.columns;
        var commandbar = config.commandbar;
        var selectable = config.selectable;
        var editable = !config.IsReadOnly;
        var events = config.events;
        config.schema = datasource.schema;
        var paging = config.paging;
        var schema = config.schema;
       
        
        var table = document.createElement('table');
        table.className = 'table table-centered table-nowrap mb-0 dataTable';
        $(table).attr('uid', table_uid);
        that.append(table);
        function refreshcell(datarow, column) {
            var td = $(`table[uid='${table_uid}'] > tbody > tr[data-id='${datarow[schema.key]}'] > td[colindex='${column.index}']`);
            if (td.length === 0) {
                return;
            }
            td.empty();
            if (!!column.template) {
                td.html(column.template(datarow));
            } else if (!!column.field) {
                td.html(datarow[column.field]);
            } else if (!!column.actions) {
                var actionsContainer = document.createElement('ul');
                actionsContainer.className = 'list-inline mb-0';
                column.actions.forEach((action) => {
                    var wrapper = document.createElement('li');
                    wrapper.className = 'list-inline-item';

                    var actionElement = $(action.template(datarow));
                    if (!!action.click) {
                        actionElement.on('click', function (e) {
                            var keyvalue = $(e.currentTarget).closest('tr').attr('data-id');
                            var dr = datasource.Data.find(d => d[schema.key] == keyvalue);
                            action.click(e, dr);
                        });
                    }

                    $(wrapper).append(actionElement);
                    actionsContainer.append(wrapper);
                });
                td.append(actionsContainer);
            }
        }
        datasource.onValueChanged = function (d, fn, fv) {
            var columnFiltered = columns.filter((c) => c.field === fn);
            columnFiltered.forEach((c) => refreshcell(d, c));
        }
        const removeAllSortState = function (but) {
            var columns = $(`table[uid='${table_uid}'] > thead > tr > th[sortstate]`).not(but);
            columns.attr('sortstate',"default");
            columns.removeClass(sortIcons.asc).removeClass(sortIcons.desc).addClass(sortIcons.default);
        };
       
        function buildColumns(thead) {
            var tr = document.createElement('tr');
            for (var i = 0; i < columns.length;i++) {
                var col = columns[i];
                col.index = i;
                var th = document.createElement('th');
                if (col.hidden === true) {
                    th.style.display = 'none';
                }
                if (!!col.renderer) {
                    th.innerHTML = th.renderer();
                } else {
                    th.innerHTML = col.name;
                }
                if (col.sortable) {
                    th.classList.add(sortIcons.default);
                    $(th).attr('sortfield', col.field);
                    $(th).attr('sortstate', 'default');
                    th.addEventListener('click', function (e) {
                        var headercell = $(e.currentTarget);
                        var state = headercell.attr('sortstate');
                        removeAllSortState(headercell);
                        switch (state) {
                            case 'asc':
                                headercell.attr('sortstate', 'desc');
                                headercell.removeClass(sortIcons.asc).addClass(sortIcons.desc);
                                break;
                            case 'desc':
                                headercell.attr('sortstate', 'default');
                                headercell.removeClass(sortIcons.desc).addClass(sortIcons.default);
                                break;
                            case 'default':
                                headercell.attr('sortstate', 'asc');
                                headercell.removeClass(sortIcons.default).addClass(sortIcons.asc);
                                break;
                        }
                        loadData();
                    });
                }
               
                tr.append(th);

            }
            thead.append(tr);
        }
       

        function buildheader(table) {
            var thead = document.createElement('thead');
            thead.classList.add('table-light');
            buildColumns(thead);
            table.append(thead);
        }
        function buildtableRowCell(tr, column, data) {
            var td = document.createElement('td');
            $(td).attr('colindex', column.index);
            if (column.hidden === true) {
                td.style.display = 'none';
            }
            if (!!column.template) {
                td.innerHTML = column.template(data);
            } else if (!!column.field) {
                td.innerHTML = data[column.field];
            } else if (!!column.actions) {
                var actionsContainer = document.createElement('ul');
                actionsContainer.className = 'list-inline mb-0';
                column.actions.forEach((action) => {
                    var wrapper = document.createElement('li');
                    wrapper.className = 'list-inline-item';

                    var actionElement = $(action.template(data));
                    if (!!action.click) {
                        actionElement.on('click', function (e) {
                            var keyvalue = $(e.currentTarget).closest('tr').attr('data-id');
                            var dr = datasource.Data.find(d => d[schema.key] == keyvalue);
                            action.click(e,dr);
                        });
                    }

                    $(wrapper).append(actionElement);
                    actionsContainer.append(wrapper);
                });
                td.append(actionsContainer);
            }
            tr.append(td);
            return td;
        }
        function buildtableRow(tbody, data) {
            var tr = document.createElement('tr');
            $(tr).attr('data-id', data[schema.key]);
            for (var i = 0; i < columns.length; i++) {
                var col = columns[i];
                


                buildtableRowCell(tr, col, data);

                
            }
            tbody.append(tr);
        };
        function buildBody(table, rows) {
            table.children("tbody").remove();
            var tbody = document.createElement('tbody');
            for (var i = 0; i < rows.length;i++) {
                var row = rows[i];
                buildtableRow(tbody, row);
            }
            table.append(tbody);
        }
        function buildPaging(container, pagingInfo) {
            pagingInfo.currentPage = pagingInfo.currentPage || 1;
            container.find('nav').remove();
            var nav = document.createElement('nav');
            nav.style.float = 'right';
            var ul = document.createElement('ul');
            ul.className = 'pagination';
            nav.append(ul);

            var previous = document.createElement('li');
            previous.classList.add('page-item');
            previous.innerHTML = ' <span class="page-link"><i class="mdi mdi-chevron-left"></i></span>';
            if (pagingInfo.currentPage === 1) {
                previous.classList.add('disabled');

            } else {
                previous.addEventListener('click', function (e) {
                    paging.currentPage = paging.currentPage - 1;
                    //buildPaging(that, paging);
                    //loaddata
                    loadData();
                })
            }
            ul.append(previous);
            //build pages
            for (var i = 1; i <= pagingInfo.TotalPages; i++) {
                var pageBtn = document.createElement('li');
                pageBtn.classList.add('page-item');
                $(pageBtn).attr("page-index", i.toString());
                if (i === pagingInfo.currentPage) {
                    pageBtn.classList.add('active');
                    pageBtn.innerHTML = `<span class="page-link">${i}<span class="sr-only">(current)</span></span>`;
                } else {
                    pageBtn.innerHTML = `<a class="page-link" href="#">${i}</a></li>`;
                    pageBtn.addEventListener("click", function (e) {
                        paging.currentPage = parseInt($(e.currentTarget).attr("page-index"));
                        //loaddata
                        loadData();
                    });

                }
                ul.append(pageBtn);
            }
            //build next

            var next = document.createElement('li');
            next.classList.add('page-item');
            next.innerHTML = ' <span class="page-link"><i class="mdi mdi-chevron-right"></i></span>';
            if (pagingInfo.currentPage === pagingInfo.TotalPages) {
                next.classList.add('disabled');
               
            } else {
                next.addEventListener('click', function (e) {
                    paging.currentPage = paging.currentPage + 1;
                    //loaddata
                    loadData();
                })
            }
            ul.append(next);
            nav.append(ul);
            container.append(nav);
        }
        function buildControl(container) {
            var table = container.children('table');
            buildheader(table);
            loadData(container);
        }
        function loadData() {
            showLoading(true);
            var sort = null;
            var container = $(`table[uid='${table_uid}']`);
             var scs = $(`table[uid='${table_uid}'] > thead > tr > th[sortstate='asc'],table[uid='${table_uid}'] > thead > tr > th[sortstate='desc']`);
            var sortedColumn = scs.length > 0 ? scs[0] : null;
            if (sortedColumn !== null) {
                sort = {
                    field: $(sortedColumn).attr('sortfield'),
                    type: $(sortedColumn).attr('sortstate')
                }
            }
            datasource.fetch(function (records, count) {
                showLoading(false);
                if (!!paging) {
                    paging.TotalPages = count / paging.size;
                    paging.TotalPages = paging.TotalPages < 1 ? 1 : Math.ceil(paging.TotalPages);
                }
                
                that.Data = records;
                buildBody(container, records);
                if (!!paging) {
                    buildPaging(that, paging);
                }
               

            }, sort, paging)
        }

        function refreshData() {
            loadData();
        }
        var that = this;
        buildControl(that);
        
        var grid = {
            element: that,
            dataSource: datasource,
            
            refresh: refreshData,
            removeRow: (rowid) => {
                var rowContainer = $(`table[uid='${table_uid}'] > tbody > tr[data-id='${rowid}']`);
                rowContainer.remove();

            },
            showRowLoading: function (rowid,display) {
                var loadingDiv = $(`div[uid='${rowid}'].overlay`);
                var rowContainer = $(`table[uid='${table_uid}'] > tbody > tr[data-id='${rowid}']`);
                if (rowContainer.length === 0) {
                    return;
                }
                if (display) {
                    if (loadingDiv.length === 0) {
                        rowContainer.append(`<div uid="${rowid}" class="overlay"><div style="position: relative;left: 50%;"  class="spinner-border text-primary m-1" role="status">
                        <span class="sr-only">Loading...</span>
                    </div></div>`);
                    } else {
                        loadingDiv.show();
                    }
                    rowContainer.addClass('overlayRow');

                } else {
                    if (loadingDiv.length > 0) {
                        loadingDiv.hide();
                        rowContainer.removeClass('overlayRow');
                    }

                }
            }
        };
        return grid;


    };

   

}(jQuery));



