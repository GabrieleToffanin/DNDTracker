FROM postgres:16 AS extension-builder

ARG PG_WAIT_SAMPLING_VERSION=v1.1.11
ARG PG_STAT_MONITOR_VERSION=2.3.2

USER root

RUN apt-get update \
    && apt-get install -y --no-install-recommends build-essential ca-certificates git postgresql-server-dev-16 \
    && git clone --depth 1 --branch "${PG_WAIT_SAMPLING_VERSION}" https://github.com/postgrespro/pg_wait_sampling.git /tmp/pg_wait_sampling \
    && make -C /tmp/pg_wait_sampling USE_PGXS=1 \
    && make -C /tmp/pg_wait_sampling USE_PGXS=1 install \
    && git clone --depth 1 --branch "${PG_STAT_MONITOR_VERSION}" https://github.com/percona/pg_stat_monitor.git /tmp/pg_stat_monitor \
    && make -C /tmp/pg_stat_monitor USE_PGXS=1 \
    && make -C /tmp/pg_stat_monitor USE_PGXS=1 install \
    && rm -rf /var/lib/apt/lists/* /tmp/pg_wait_sampling /tmp/pg_stat_monitor

FROM postgres:16

COPY --from=extension-builder /usr/lib/postgresql/16/lib/pg_wait_sampling.so /usr/lib/postgresql/16/lib/pg_wait_sampling.so
COPY --from=extension-builder /usr/lib/postgresql/16/lib/pg_stat_monitor.so /usr/lib/postgresql/16/lib/pg_stat_monitor.so
COPY --from=extension-builder /usr/share/postgresql/16/extension/pg_wait_sampling* /usr/share/postgresql/16/extension/
COPY --from=extension-builder /usr/share/postgresql/16/extension/pg_stat_monitor* /usr/share/postgresql/16/extension/

CMD ["postgres", "-c", "shared_preload_libraries=pg_stat_statements,pg_wait_sampling,pg_stat_monitor", "-c", "pg_stat_statements.max=10000", "-c", "pg_stat_statements.track=all", "-c", "pg_stat_statements.save=on"]
